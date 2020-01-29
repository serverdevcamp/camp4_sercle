import socket
from _thread import *

def threaded(client_socket, addr):
    print('Connected by: ', addr[0], ' : ', addr[1])

    while True:

        try:
            data = client_socket.recv(1024)

            if not data:
                print('Disconnected by ' + addr[0], ':', addr[1])
                break

            print('Received from ' + addr[0],':',addr[1], data.decode())

            client_socket.send(data)

        except ConnectionResetError as e:
            print('Disconnected by ' + addr[0],':', addr[1])
            break
    client_socket.close()


def clientthread(conn, addr):
    #conn.send("Welcome to this chatroom!".encode())
    print("thread 생성")
    while True:
        try:
            message = conn.recv(1024)
            if not message:
                remove(conn)

            print("<" + str(addr[1]) + ">" )

            broadcast(message, addr[1])
        except:
            continue

def broadcast(message, connection):
    for clients in list_of_clients:
        if clients[1] != connection:
            clients[0].send(message)
 
            

def remove(connection):
    if connection in list_of_clients:
        list_of_clients.remove(connection)



HOST = '127.0.0.1'
PORT = 3098

#소켓 객체 생성, IPv4, TCP
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# 포트사용중이라 연결할 수 없다는 WInError 10048 에러 해결위해 필요
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
# 네이글 알고리즘 해제
server_socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)

# bind는 소켓을 특정 네트워크 인터페이스와 포트 번호에 연결하는데 사용
#Host는 hostname, ip address, "" 이 될 수 있음
# 빈문자열이면 모든 네트워크 인터페이스로부터의 접속을 허용함
# 포트는 1~65535 까지 가능
server_socket.bind((HOST, PORT))

server_socket.listen()


list_of_clients = []

print('server start')

while True:
    print('waiting new client..')
    client_socket, addr = server_socket.accept()
    # addr[1]은 포
    list_of_clients.append([client_socket, addr[1]])

    print(addr[0] + ' ' + str(addr[1]) + ' connected')
    start_new_thread(clientthread, (client_socket, addr))
    
    
server_socket.close()
