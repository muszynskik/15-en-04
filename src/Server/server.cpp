#include <cstdio>
#include <cstdlib>
#include <unistd.h>
#include <errno.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>
#include <arpa/inet.h>
#include <sys/wait.h>
#include <signal.h>

#include <libxml/parser.h>
#include <libxml/tree.h>

#define PORT "6666"  // the port users will be connecting to

#define BACKLOG 10     // how many pending connections queue will hold

int new_fd;
 
static void set(xmlNode * Node, char *What, char *Text)
{
    xmlNode *curNode = NULL;
    for (curNode = Node; curNode; curNode = curNode->next) {
        if (curNode->type == XML_ELEMENT_NODE) {
			// xmlStrcmp returns the integer result of the comparison
            if (xmlStrcmp(curNode->name, (const xmlChar*) What) == 0) {
                xmlNodePtr node1 = xmlNewText(BAD_CAST Text);
                xmlAddChild(curNode, node1);
            }
        }
        set(curNode->children, What, Text);
    }
}

void reverse(char s[])
 {
     int i, j;
     char c;
 
     for (i = 0, j = strlen(s)-1; i<j; i++, j--) {
         c = s[i];
         s[i] = s[j];
         s[j] = c;
     }
 }

void itoa(int n, char s[])
 {
     int i, sign;
 
     if ((sign = n) < 0)  /* record sign */
         n = -n;          /* make n positive */
     i = 0;
     do {       /* generate digits in reverse order */
         s[i++] = n % 10 + '0';   /* get next digit */
     } while ((n /= 10) > 0);     /* delete it */
     if (sign < 0)
         s[i++] = '-';
     s[i] = '\0';
     reverse(s);
}
 
void sigchld_handler(int s)
{
    while(waitpid(-1, NULL, WNOHANG) > 0);
}

// Function to get sockaddr
void *get_in_addr(struct sockaddr *sa)
{
    if (sa->sa_family == AF_INET) {
        return &(((struct sockaddr_in*)sa)->sin_addr);
    }

    return &(((struct sockaddr_in6*)sa)->sin6_addr);
}

// Function to send register response
void SendRegisterResponse()
{
    printf("SendRegisterResponse() called.\n");
    static long int idCounter = 1;

	// Attempt to parse an XML file
    xmlDoc *doc = NULL;
    doc = xmlReadFile("xml/registerResponse.xml", NULL, 0);
    if (doc == NULL) {
        fprintf(stderr, "Failed to parse document\n");
        exit(1);
    }
    
    char tmp[50] = {0};

	// Attempt to get the root element (because a file is parsed to a tree)
    xmlNode *root = NULL;
    root = xmlDocGetRootElement(doc);

    itoa(idCounter, tmp);
    set(root, "Id", tmp);
    memset(tmp, 0, 50);
    idCounter++;
    
//example timeout
    
    snprintf(tmp, 50, "00:00:05.000+01:00");
    set(root, "Timeout", tmp);
    
	// Dump the document to a buffer and print in for demonstration purposes
    xmlChar *xmlbuff;
    int buffersize;
    xmlDocDumpFormatMemory(doc, &xmlbuff, &buffersize, 1); 
    
    // Register with the server
    printf("Server is sending Register Response...\n");
    send(new_fd, xmlbuff, buffersize, 0);
}

// Function to send solve request response
void SendSolveRequestResponse(){// right now we are not using it 

	printf("SendSolveRequestResponseResponse() called.\n");

	// Attempt to parse an XML file
    xmlDoc *doc = NULL;
    doc = xmlReadFile("xml/solveRequestResponse.xml", NULL, 0);
    if (doc == NULL) {
        fprintf(stderr, "Failed to parse document\n");
        exit(1);
    }
    
	// Dump the document to a buffer and print in for demonstration purposes
    xmlChar *xmlbuff;
    int buffersize;
    xmlDocDumpFormatMemory(doc, &xmlbuff, &buffersize, 1);
 
    printf("Server is sending solveRequestResponse\n");
    send(new_fd, xmlbuff, buffersize, 0);
}

// Function to parse message
void parseMsg(char* buffer){

			// Parse an XML in-memory document and build a tree
			xmlDoc *doc; /* the resulting document tree */
            doc = xmlReadMemory(buffer, 1024, "registerMsg.xml", NULL, 0);
            if (doc == NULL) {
                fprintf(stderr, "Failed to parse document\n");
                exit(1);
            }
    
			// Attempt to get the root element of the document
            xmlNode *root = NULL;
            root = xmlDocGetRootElement(doc);
            printf("Root: %s\n", root->name);
			// xmlStrcmp returns the integer result of the comparison
            if (xmlStrcmp(root->name, BAD_CAST "SolveRequest") == 0)
            {
                printf("Server received SolveRequest msg.\n");
                
              //  SendSolveRequestResponse();
            }


}

// Function to wait for messages
 void waitForMsg(char* buffer){

		int byteCounter;
 
		while(1){
		memset(buffer, 0, 1024);
		byteCounter = recv(new_fd, buffer, 1024, 0);
		   
		   if(byteCounter>0){
		   printf("\nServer has received %d bytes:\n%s\n\n", byteCounter, buffer);
		   parseMsg(buffer);
		   break;
		   }else 
		   continue;
		 }

		 
waitForMsg(buffer);

}


int main(void)
{
    int sockfd;  // Listen on sock_fd, new connection on new_fd
    struct addrinfo hints, *servinfo, *p;
    struct sockaddr_storage their_addr; // Connector's address information
    socklen_t sin_size;
    struct sigaction sa;
    int yes=1;
    char s[INET6_ADDRSTRLEN];
    int rv;

    memset(&hints, 0, sizeof hints);
    hints.ai_family = AF_UNSPEC;
    hints.ai_socktype = SOCK_STREAM;
    hints.ai_flags = AI_PASSIVE; // Use my IP

	// Success returns zero. Failure returns a nonzero Windows Sockets error code
    if ((rv = getaddrinfo(NULL, PORT, &hints, &servinfo)) != 0) {
        fprintf(stderr, "getaddrinfo: %s\n", gai_strerror(rv));
        return 1;
    }

    // Loop through all the results and bind to the first we can
    for(p = servinfo; p != NULL; p = p->ai_next) {

		// Creates a socket that is bound to a specific transport service provider
        // On success zero. On error -1
        if ((sockfd = socket(p->ai_family, p->ai_socktype,
                p->ai_protocol)) == -1) {
            perror("server: socket");
            continue;
        }

		// Set a socket option
        // On success zero. On error -1
        if (setsockopt(sockfd, SOL_SOCKET, SO_REUSEADDR, &yes,
                sizeof(int)) == -1) {
            perror("setsockopt");
            exit(1);
        }

		// Bind a name to a socket
        // On success zero. On error -1
        if (bind(sockfd, p->ai_addr, p->ai_addrlen) == -1) {
            close(sockfd);
            perror("server: bind");
            continue;
        }

        break;
    }

    if (p == NULL)  {
        fprintf(stderr, "server: failed to bind\n");
        return 2;
    }

	// Free address information
    freeaddrinfo(servinfo); // All done with this structure

	// Listen for connections on a socket
    // On success zero. On error -1
    if (listen(sockfd, BACKLOG) == -1) {
        perror("listen");
        exit(1);
    }

    sa.sa_handler = sigchld_handler; // Reap all dead processes
    sigemptyset(&sa.sa_mask);
    sa.sa_flags = SA_RESTART;

	// Examine and change a signal action
    // On success zero. On error -1
    if (sigaction(SIGCHLD, &sa, NULL) == -1) {
        perror("sigaction");
        exit(1);
    }

    printf("server: waiting for connections...\n");

    while(1) {  // main accept() loop
        sin_size = sizeof their_addr;

		// Accept a connection on a socket
        // On success zero. On error -1
        new_fd = accept(sockfd, (struct sockaddr *)&their_addr, &sin_size);
        if (new_fd == -1) {
            perror("accept");
            continue;
        }

		// Convert IPv4 and IPv6 addresses from binary to text form
        inet_ntop(their_addr.ss_family,
            get_in_addr((struct sockaddr *)&their_addr),
            s, sizeof s);
        printf("server: got connection from %s\n", s);

        if (!fork()) { // This is the child process
            close(sockfd); // Child doesn't need the listener
            //if (send(new_fd, "Hello, world!", 13, 0) == -1)
            //    perror("send");
            char *buffer = (char*) malloc(1024);
            memset(buffer, 0, 1024);
            printf("Server is receiving...\n");
            int byteCount = 0;
            
            byteCount = recv(new_fd, buffer, 1024, 0);
            
            printf("\nServer has received %d bytes:\n%s\n\n", byteCount, buffer);
            
            // Parse an XML in-memory document and build a tree.
            xmlDoc *doc; // The resulting document tree
            doc = xmlReadMemory(buffer, 1024, "registerMsg.xml", NULL, 0);
            if (doc == NULL) {
                fprintf(stderr, "Failed to parse document\n");
                return 1;
            }
    
			// Get the root element of the document 
            xmlNode *root = NULL;
            root = xmlDocGetRootElement(doc);
            printf("Root: %s\n", root->name);
			// xmlStrcmp returns the integer result of the comparison
            if (xmlStrcmp(root->name, BAD_CAST "Register") == 0)
            {
                printf("Server received REGISTER msg.\n");
                
                SendRegisterResponse();
            }
	 

			waitForMsg(buffer);
		 		 
		 
            close(new_fd);
            free(buffer);
            exit(0);
        }
        close(new_fd);  // parent doesn't need this
    }

  // Cleanup function for the XML library
    xmlCleanupParser();

    // Debug memory for regression tests
    xmlMemoryDump();
	
    return 0;
}