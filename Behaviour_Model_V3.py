# This version use Socket, and connect to C#, and be controlled by POSH
#Later Imporvement 1: Using sensory input from remote control
#Later Imporvement 2: Pacakge different funcitons into sepsarate py files
#                     eg, Socket, Behaviour Functions
import array
from movement_generator import MovementGenerator
from time import gmtime, strftime
import numpy as np
import random as rm
import time
from RingBuffer import RingBuffer
import socket

# IMPORTANT: Power the Raspberry Pi with a power supply of at least 2.1A to prevent the SSH connection from shutting off!
#remoteLog = open('/home/pi/temp/remote.log','w')

mover = MovementGenerator('all_keypoints.json',0.02,2)

# A ring buffer to store history for last 20 behvaiours
Buffer = RingBuffer(20)

class Socket():

    def __init__(self):

        # next create a socket object
        s = socket.socket()
        print("Socket successfully created")

        # reserve a port on computer in our
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

        # Establish connection with client.
        c, addr = s.accept()
        print('Got connection from', addr)

    #Recieve data from socket
    def recieve(self):
        # recieve data  30 Bytes
        recv_data = s.recv(30).decode("gbk")
        #recv_data = recv_data.decode('gbk')

        print('RECIVED DATA:', recv_data)

        return recv_data

    #send data through socket
    def send(self,data):
        s.send(data.encode("gbk"))
        print('SEND DATA:',data)

#The dictionary of all behaviours
BehavDict = {"Reset":0,"Forward":1,"TurnLeft":2,"TurnRight":3,"Iddle":4,
            "Backwards":5,"Cuddle":6,"Mothering":7,"Escape":8,"Greeting":9,
            "Seperation":10,"Surprise":11,"SocialCall":12,"Caress":12}

#Functions of behaviours
def Reset():
    mover.reset_positions()
    return

def Forward():
    print('Forward')
    mover.generate_motion(0, 150)
    return

def TurnLeft():
    print('Left')
    mover.generate_motion(150, 300)
    return

def TurnRight():
    print('Right')
    mover.generate_motion(300, 450)
    return

def Iddle():
    print('Iddle')
    mover.generate_motion(450, 540)
    return

def Backwards():
    print('Backwards')
    mover.generate_motion(540, 690)
    return

def Cuddle():
    print('Cuddle')
    mover.generate_motion(690, 850)
    return

def Mothering():
    print('Mothering')
    mover.generate_motion(850, 930)
    return

def Escape():
    print('Escape')
    mover.generate_motion(930, 1010)
    return

def Greeting():
    print('Greeting')
    mover.generate_motion(1010, 1100)
    return

def Seperation():
    print("Seperation")
    mover.generate_motion(1100,1260)

def Surprise():
    print('Surprise')
    mover.generate_motion(1260, 1330)
    return

def SocialCall():
    print('Social Call')
    mover.generate_motion(1330, 1500)
    return

def Caress():
    print('Caress')
    mover.generate_motion(1500, 1610)
    return

# BehavFreq returns a list of frequencies
# the index of the list is map to the behaviour according to BehavDict

# Find a better way to calculate the frequencies of each behaviour
# Maybe only update the value of latest and oldest data?
# ==> in this case, maybe add another parameter of offset for buffer.read(),
#     in order to read specific latest and oldest data

def BehavFreq(Buffer):

    freq = [0]*len(BehavDict)

    # go through the entire ringbuffer everytime
    for i in range(0, Buffer.capacity):
        #check if data in buffer is a valid behaviour
        if Buffer.read() in BehavDict:
            index = BehavDict[Buffer.read()]
            freq[index] += 1

    #this will change int quantity into freq
    #for i in range(0,len(freq)):
    #    freq[i] /= Buffer.capacity

    return freq

def main():
    #instanize a Socket
    Connection = Socket()

    while True:
        #Get the behaviour name to execute from Socket
        command = Connection.recieve()
        if command in BehavDict:
            # execute the behaviour
            command()
            #send "Done" flag of behviour completion
            Connection.send("Done")

            #write the behaviour to buffer
            Buffer.write(command)

            #calculate every freq.
            freq = BehavFreq(Buffer)

            #py.sleep(10)
            #maybe find way to sleep, or better way to do with asychronize communicaiton

            #send a start flag before sending frequncies
            Connetion.send("Start")
            #send the frequncies one by one
            for i in freq:
                Connetion.send(i)

            # send an End flag after sending frequncies
            Connection.send("End")

        else:
            Connection.send("Unrecognised command")

        command = []



if __name__ == "__main__":
    main()