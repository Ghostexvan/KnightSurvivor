import socket

host, port = "127.0.0.1", 25001
data = "Move"

# SOCK_STREAM means TCP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

while True:
    try:
        # Connect to the server and send the data
        sock.connect((host, port))
        sock.sendall(data.encode("utf-8"))
        response = sock.recv(1024).decode("utf-8")
        print (response)

    finally:
        sock.close()
        break