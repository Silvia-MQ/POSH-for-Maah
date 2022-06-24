from socket import *

Behaviours = ['Forward', 'Backward', 'Left', 'Right', 'Social Call',
              'Surpirse', 'Mothering', 'Iddle', 'Greeting', 'Caress',
              'Escape', 'Cuddle', 'Seperation']
#print(len(Behaviours))
print(type(Behaviours[0]))
# Create Socket
tcp_server_socket = socket(AF_INET, SOCK_STREAM)
tcp_server_socket.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)

# ip address
address = ('', 8885)
# bind
tcp_server_socket.bind(address)

# instead of take the initiative, use listen to recieve connection
tcp_server_socket.listen(128)

# if there is a client want to connect, create a specific for this client
# client_socket is specific for this client
# tcp_server_socket could wair for other connections
client_socket, clientAddr = tcp_server_socket.accept()
print('TCP Connected')

# recieve data
recv_data = client_socket.recv(30)
recv_data=recv_data.decode('gbk')

print('RECIVED DATA:', recv_data)

if (recv_data == 'Behaviour tree required'):
    # send behaviors tree
    for i in Behaviours:
        client_socket.send(i.encode("gbk"))
        client_socket.send(' '.encode("gbk"))
    print('Behaviours Config is sent')
elif (len(recv_data) != 0):
    send_data = "Done"
    client_socket.send(send_data.encode("gbk"))
    print('SENT DATA：',send_data)


