import asyncio
from _thread import *

HOST = '0.0.0.0'
PORT = 3000

list_of_clients = []
print('Chatting server start')


async def chatting_handle(reader, writer):  # 각 소켓
    addr = writer.get_extra_info('peername')   # 0: IP 1: PORT
    print(str(addr[0]) + " " + str(addr[1]) + "connect")
    user_name = await reader.read(100)
    list_of_clients.append([writer, user_name])
    client = list_of_clients[-1]

    while True:
        try:
            message = await reader.read(100)
            if not message:
                remove(client)
                break

            message = user_name + " : ".encode() + message
            print(message)
            for user in list_of_clients:
                user[0].write(message)
                await user[0].drain()
        except Exception as e:
            print(e)
            continue


async def main():
    server = await asyncio.start_server(chatting_handle, HOST, PORT)

    address = server.sockets[0].getsockname()
    print({address})

    async with server:
        await server.serve_forever()


def remove(connection):
    for user in list_of_clients:
        if user is connection:
            list_of_clients.remove(user)
            print(connection[1].decode() + "님이 나가셨습니다.")
            break


asyncio.run(main())
