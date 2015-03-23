Cygwin: libxml2 libxml2-devel gcc gcc-g++ 
g++ ./client.cpp -o client -I/usr/include/libxml2 -L/usr/lib/ -lxml2
g++ ./server.cpp -o server -I/usr/include/libxml2 -L/usr/lib -lxml2
