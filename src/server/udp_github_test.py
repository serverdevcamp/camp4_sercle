import socket
from _thread import *

HOST = '127.0.0.1'
PORT = 3098

# 소켓 객체 생성, IPv4, TCP
server_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
# 포트사용중이라 연결할 수 없다는 WInError 10048 에러 해결위해 필요
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

# bind는 소켓을 특정 네트워크 인터페이스와 포트 번호에 연결하는데 사용
# Host는 hostname, ip address, "" 이 될 수 있음
# 빈문자열이면 모든 네트워크 인터페이스로부터의 접속을 허용함
# 포트는 1~65535 까지 가능
server_socket.bind((HOST, PORT))

list_of_clients = []

print('server start')

while True:
    data, addr = server_socket.recvfrom(1024)

    if addr not in list_of_clients:
        list_of_clients.append(addr)
        print("client logged in.")
    for client in list_of_clients:
        if addr != client and len(data) > 0:
            server_socket.sendto(data, client)
            print(addr[0] + ' ' + str(addr[1]) + "> " )
server_socket.close()
