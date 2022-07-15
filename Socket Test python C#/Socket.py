#this file is a test of socket communication
# Notes: run this python file before run C#
#        first communication must be sent from C#, recevied by python

# first of all import the socket library
import socket
import json

#test json message
msg = {
    "Fans":1,
    "Raid":1,
    "boardTemperature":66,
    "Power":1
}
# next create a socket object
s = socket.socket()
print("Socket successfully created")

# reserve a port on your computer in our
# case it is 12345 but it can be anything
port = 12345

# Next bind to the port
# we have not typed any ip in the ip field
# instead we have inputted an empty string
# this makes the server listen to requests
# coming from other computers on the network
s.bind(('', port))
print("socket binded to %s" % (port))

# put the socket into listening mode
s.listen(5)
print("socket is listening")

# a forever loop until we interrupt it or
# an error occurs
while True:
    # Establish connection with client.
    c, addr = s.accept()
    print('Got connection from', addr)

    # send a thank you message to the client. encoding to send byte type.
    #c.sendall('Thank you for connecting'.encode(encoding='UTF-8'))
    print('message received:', c.recv(1024).decode(encoding='UTF-8'))


    sendmsg1 = json.dumps('message received:')
    flag = c.sendall(sendmsg1.encode(encoding='UTF-8'))
    #time.sleep(10)
    #print('message 222 received:', c.recv(1024).decode(encoding='UTF-8'))
    sendmsg = json.dumps(msg) #< -- change dict into string
    #time.sleep(10)
    #sendmsg = json.dumps('message received:')
    flag1 = c.sendall(sendmsg.encode(encoding='UTF-8')) #<-- encode string into bytes

    print("Flag", flag,"Flag1",flag1)
    # Close the connection with the client
    #c.close()

    # Breaking once connection closed
    break