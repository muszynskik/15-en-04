#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <errno.h>
#include <string.h>
#include <netdb.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <arpa/inet.h>

#include <libxml/parser.h>
#include <libxml/tree.h>

#define PORT "6666" // the port client will be connecting to 
#define MAXDATASIZE 100 // max number of bytes we can get at once 


int sockfd;  
	
// Function to set type
static void setType(xmlNode * Node, char *Text)
{
    xmlNode *curNode = NULL;
    for (curNode = Node; curNode; curNode = curNode->next) {
        if (curNode->type == XML_ELEMENT_NODE) {
			// xmlStrcmp returns the integer result of the comparison
            if (xmlStrcmp(curNode->name, (const xmlChar*)"Type") == 0) {
                xmlNodePtr node1 = xmlNewText(BAD_CAST Text);
                xmlAddChild(curNode, node1);
            }
        }
        setType(curNode->children, Text);
    }
}

//Function to set problem name
static void setProblemName(xmlNode * Node, char *Text)
{
    xmlNode *curNode = NULL;
    for (curNode = Node; curNode; curNode = curNode->next) {
        if (curNode->type == XML_ELEMENT_NODE) {
            // xmlStrcmp returns the integer result of the comparison
            if (xmlStrcmp(curNode->name, (const xmlChar*)"ProblemName") == 0) {
                xmlNodePtr node1 = xmlNewText(BAD_CAST Text);
                xmlAddChild(curNode, node1);
            }
        }
        setProblemName(curNode->children, Text);
    }
}

// Function to set parallel threads
static void setPT(xmlNode * Node, char *Text)
{
    xmlNode *curNode = NULL;
    for (curNode = Node; curNode; curNode = curNode->next) {
        if (curNode->type == XML_ELEMENT_NODE) {
			// xmlStrcmp returns the integer result of the comparison
            if (xmlStrcmp(curNode->name, (const xmlChar*)"ParallelThreads") == 0) {
                xmlNodePtr node1 = xmlNewText(BAD_CAST Text);
                xmlAddChild(curNode, node1);
            }
        }
        setPT(curNode->children, Text);
    }
}

// Function to get sockaddr
void *get_in_addr(struct sockaddr *sa)
{
    if (sa->sa_family == AF_INET) {
        return &(((struct sockaddr_in*)sa)->sin_addr);
    }
    return &(((struct sockaddr_in6*)sa)->sin6_addr);
}

// Function to receive client register response message
void receiveClientRegisterResponseMsg()
{
            char *buffer = (char*) malloc(1024);
            memset(buffer, 0, 1024);
            printf("Client is receiving...\n");
            int byteCount = 0;
            // Receive a message from a socket
            byteCount = recv(sockfd, buffer, 1024, 0);
			
            printf("\nClient has received %d bytes:\n%s\n\n", byteCount, buffer);
}

// Function to send message
void sendMsg(char*  name)
{
    // Attempt to parse an XML file
    xmlDoc *doc = NULL;
    doc = xmlReadFile(name, NULL, 0);
    if (doc == NULL) {
        fprintf(stderr, "Failed to parse document\n");
        exit(1);
    }
    
    // Dump the document to a buffer and print it for demonstration purposes
    xmlChar *xmlbuff;
    int buffersize;
    xmlDocDumpFormatMemory(doc, &xmlbuff, &buffersize, 1);
 
    printf("Client is sending %s\n", name);
    // Transmit a message to another socket
    send(sockfd, xmlbuff, buffersize, 0);
}

// Function to waint for response
void waitForResponse()
{
		int byteCounter=0;
		char *buffer = (char*) malloc(1024);
    while(1) {
        memset(buffer, 0, 1024);
		byteCounter = recv(sockfd, buffer, 1024, 0);
		   
		   if(byteCounter>0) {
		   printf("\nClient has received %d bytes:\n%s\n\n", byteCounter, buffer);
		   break;
           }
    }
}


void consoleMenu()
{
    int option;
      
     printf("\n Menu:\n\n");
     printf(" 1. Send Status Msg\n");
     printf(" 2. Send Solve Request Msg\n");
     printf(" 3. Send Solutions Msg\n");
	 printf(" 4. Send Solution Request Msg \n");
     printf("\n Selection?\n\n");
     scanf ("%d", &option);

	 printf("\n %d \n\n", option);
	 
     switch(option)
     {
        case 1: 
             sendMsg("xml/statusMsg.xml");
             break;
			 
		 case 2: 
             sendMsg("xml/solveRequestMsg.xml");
			 // waitForResponse(); we are not waiting right now 
             break;
		
		 case 3: 
             sendMsg("xml/solutionsMsg.xml");
             break;
			 
		case 4: 
             sendMsg("xml/solutionRequestMsg.xml");
             break;
		
		 default:
            printf ("Bad input \n");                
            break;  
	}
	consoleMenu();
} 


//*
// main
//*
int main(int argc, char *argv[])
{
    /*
     * this initialize the library and check potential ABI mismatches
     * between the version it was compiled for and the actual shared
     * library used.
     */
    LIBXML_TEST_VERSION
    
    char buf[MAXDATASIZE];
    struct addrinfo hints, *servinfo, *p;
    int rv;
    char s[INET6_ADDRSTRLEN];

    // Instruction of usage
    if (argc != 2) {
        fprintf(stderr,"usage: client hostname\n");
        exit(1);
    }

    memset(&hints, 0, sizeof hints); 
    hints.ai_family = AF_UNSPEC;
    hints.ai_socktype = SOCK_STREAM;

    if ((rv = getaddrinfo(argv[1], PORT, &hints, &servinfo)) != 0) {
        fprintf(stderr, "getaddrinfo: %s\n", gai_strerror(rv));
        return 1;
    }

    // Loop through all the results and connect to the first we can
    for(p = servinfo; p != NULL; p = p->ai_next) {
        if ((sockfd = socket(p->ai_family, p->ai_socktype,
                p->ai_protocol)) == -1) {
            perror("client: socket");
            continue;
        }

        if (connect(sockfd, p->ai_addr, p->ai_addrlen) == -1) {
            close(sockfd);
            perror("client: connect");
            continue;
        }

        break;
    }
    
    // Can't find any result
    if (p == NULL) {
        fprintf(stderr, "client: failed to connect\n");
        return 2;
    }

    // Convert IPv4 and IPv6 addresses from binary to text form
    inet_ntop(p->ai_family, get_in_addr((struct sockaddr *)p->ai_addr),
            s, sizeof s);
    printf("client: connecting to %s\n", s);

    // Free address information
    freeaddrinfo(servinfo); // all done with this structure

    // Attempt to parse an XML file
    xmlDoc *doc = NULL;
    doc = xmlReadFile("xml/registerMsg.xml", NULL, 0);
    if (doc == NULL) {
        fprintf(stderr, "Failed to parse document\n");
        return 1;
    }
    
    // Attempt to get the root element (because a file is parsed to a tree)
    xmlNode *root = NULL;
    root = xmlDocGetRootElement(doc);

    setType(root, "Task Manager");
    setProblemName(root, "Problem 1");
    setPT(root, "2");

    // Dump the document to a buffer and print in for demonstration purposes
    xmlChar *xmlbuff;
    int buffersize;
    xmlDocDumpFormatMemory(doc, &xmlbuff, &buffersize, 1);
    printf("%s", (char *) xmlbuff);
 
    // Register with the server
    printf("Client is sending...\n");
    send(sockfd, xmlbuff, buffersize, 0);

    // Wait (recv()) for register response
	receiveClientRegisterResponseMsg();
	    
    // If register response has been received allow to send rest of the messages
	consoleMenu();

    //
    close(sockfd);

    // Cleanup function for the XML library
    xmlCleanupParser();

    // Debug memory fo regression tests
    xmlMemoryDump();
    
    return 0;
}