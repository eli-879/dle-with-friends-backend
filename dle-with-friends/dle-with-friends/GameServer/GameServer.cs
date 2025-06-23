using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DleWithFriends.GameServer;

public class GameServer
{
    private TcpListener _tcpListener;
    private readonly int _port = 5000;

    public async Task StartServer()
    {
        try
        {
            _tcpListener = new TcpListener(IPAddress.Any, _port);

            _tcpListener.Start();

            Console.WriteLine("Server started!");

            while (true)
            {
                TcpClient client = await _tcpListener.AcceptTcpClientAsync();

                Console.WriteLine("Client connected");

                HandleClientAsync(client);
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine("SocketException: {0}", ex);

        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        NetworkStream stream = client.GetStream();

        try
        {
            byte[] buffer = new byte[1024];
            string receivedMessage;

            int readTotal;

            while ((readTotal = await stream.ReadAsync(buffer)) != 0)
            {
                receivedMessage = Encoding.UTF8.GetString(buffer, 0, readTotal);
                Console.WriteLine($"Received Message: {receivedMessage}");

                var receivedResponse = $"Received your message of: {receivedMessage}";
                var receivedResponseEncoded = Encoding.UTF8.GetBytes(receivedResponse);

                await stream.WriteAsync(receivedResponseEncoded);
                Console.WriteLine($"Sent: {receivedResponse}");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client disconnected");

        }
    }
}
