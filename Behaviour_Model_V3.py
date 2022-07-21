# This version use Socket, and connect to C#, and be controlled by POSH

#Later Imporvement 1: Using sensory input from remote control
#^ Done. Take OK Button as input
#Further Imporvement: Multiple input as different meaning

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
import json
import usb.core, usb.util, usb.control

# IMPORTANT: Power the Raspberry Pi with a power supply of at least 2.1A to prevent the SSH connection from shutting off!
#remoteLog = open('/home/pi/temp/remote.log','w')

mover = MovementGenerator('all_keypoints.json',0.02,2)

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
    def getdata(self):
        # recieve data for 30 Bytes
        recv_data = s.recv(30).decode(encoding='UTF-8')

        print('RECIVED DATA:', recv_data)

        return recv_data

    #send data through socket
    def pushdata(self,data):
        s.sendall(data.encode(encoding='UTF-8'))
        print('SEND DATA:',data)

#The dictionary of all behaviours
BehavDict = {"Reset":0,"Forward":0,"TurnLeft":0,"TurnRight":0,"Iddle":0,
            "Backwards":0,"Cuddle":0,"Mothering":0,"Escape":0,"Greeting":0,
            "Seperation":0,"Surprise":0,"SocialCall":0,"Caress":0}
#The dictionary of Peting status
# Pet Now is either 1 or 0
# Pet Past is integer
PetDict = {"PetNow":0,"PetPast":0}

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

# BehavFreq returns a list of counts of each behaviour in memory of 20 behaviours

# Possible Imporvement: Find a better way to calculate the frequencies of each behaviour
# Maybe only update the value of latest and oldest data?
# ==> in this case, maybe add another parameter of offset for buffer.read(),
#     in order to read specific latest and oldest data

def BehavFreq(Buffer):

    freq = BehavDict.copy()

    # go through the entire ringbuffer everytime
    for i in range(0, Buffer.capacity):
        #check if data in buffer is a valid behaviour
        if Buffer.read() in BehavDict:
            freq[Buffer.read()] += 1

    #this will change counts into freq
    #for i in range(0,len(freq)):
    #    freq[i] /= Buffer.capacity

    return freq

def PetFreq(sensor, Pet):

    freq = PetDict.copy()
    if (sensor == 1):
        freq["PetNow"] = 1
        Pet.write(1)
    else:
        freq["PetNow"] = 0
        Pet.write(0)

    # go through the entire ringbuffer everytime
    for i in range(0, Buffer.capacity):
        #check if data in buffer is a valid behaviour
        if Buffer.read()==1:
            freq["PetPast"] += 1

    return freq


class Remote():

    def __init__(self):
        # initiate remote
        dev = usb.core.find(idVendor=0x2252, idProduct=0x1037)
        execute = None
        try:
            if dev is None:
                raise ValueError('device not found')

            cfg = dev.get_active_configuration()

            # Create remote interfaces. The remote seems to need 2 interfaces to transmit a value from 0 to 256:
            interface_number1 = cfg[(0, 0)].bInterfaceNumber  # used for the first interface
            interface_number2 = cfg[(1, 0)].bInterfaceNumber  # used for the second interface

            intf1 = usb.util.find_descriptor(
                cfg, bInterfaceNumber=interface_number1)
            intf2 = usb.util.find_descriptor(
                cfg, bInterfaceNumber=interface_number2)

            if dev.is_kernel_driver_active(interface_number1):
                dev.detach_kernel_driver(interface_number1)
            if dev.is_kernel_driver_active(interface_number2):
                dev.detach_kernel_driver(interface_number2)

            ep1 = usb.util.find_descriptor(
                intf1,
                custom_match=lambda e: usb.util.endpoint_direction(e.bEndpointAddress) == usb.util.ENDPOINT_IN)
            ep2 = usb.util.find_descriptor(
                intf2,
                custom_match=lambda e: usb.util.endpoint_direction(e.bEndpointAddress) == usb.util.ENDPOINT_IN)


    # check if the OK button is pressed or not
    def checkbutton(self):
        try:
            # lsusb -v : find wMaxPacketSize (8 in my case)
            remote1 = ep1.read(3, timeout=50)
            signal = int(remote1[2])
            if signal == 40:
                return 1
        except usb.core.USBError:
            pass

        try:
            remote2 = ep2.read(3, timeout=50)
            signal = int(remote2[1])
            if signal ==40:
                return 1
        except usb.core.USBError:
            pass
        return 0

def main():
    #ring buffers to store history for last 20 behvaiours
    Behav = RingBuffer(20)
    Pet = RingBuffer(20)

    #instanize a Socket
    Connection = Socket()
    sensor = Remote()

    while True:
        #check for remote input
        Sense_input = sensor.checkbutton()

        #Get the behaviour name to execute from Socket
        #only send behaviour frequencies and pet status when a command recieved and executed
        command = Connection.getdata()
        if command in BehavDict:
            # execute the behaviour
            command()

            #write the behaviour to buffer
            Behav.write(command)

            #calculate every freq.
            B_freq = BehavFreq(Behav)

            #send the frequncies
            # change dict into string
            flag_Behav = c.pushdata(json.dumps(B_freq))
            # flag is None if sending succeed
            if flag_Behav!=None:
                print("Errors at sending Behaviour data to C#")

            P_freq = PetFreq(Sense_input, Pet)

            # send the pet status
            # change dict into string
            flag_Pet = c.pushdata(json.dumps(P_freq))
            if flag_Pet != None:
                print("Errors at sending Pet data to C#")

        else:
            Connection.pushdata('Unrecognised command')


        command.clear()



if __name__ == "__main__":
    main()