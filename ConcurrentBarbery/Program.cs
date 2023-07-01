using System.Collections.Concurrent;

Console.WriteLine("Barber shop!");

int barbers = 2;
bool isBarberShopClosed = false;

ConcurrentQueue<Client?> clients = new ConcurrentQueue<Client?>() { };

foreach (int i in Enumerable.Range(1, 10))
{
    NewClient(i);
}

await Task.WhenAll(AttendClients(), MoreClients());

void NewClient(int number)
{
    Random random = new Random();
    Console.WriteLine($"New client {number} arrival!");
    clients.Enqueue(new Client($"Client {number}", random.Next(1,10)));
}

async Task CutHair(Client client)
{
    Console.WriteLine($"The {client.Name} started to cut his hair.");
    await Task.Delay(client.TimeInSeconds * 1000);
    Console.WriteLine($"The {client.Name} finished in {client.TimeInSeconds} seconds.");
}

async Task AttendClients()
{
    List<Task> workingBarbers = new List<Task>();
    while(clients.Any() || workingBarbers.Count > 0)
    {
        while(workingBarbers.Count < barbers && clients.TryDequeue(out Client client))
        {
            Task barberTask = CutHair(client);
            workingBarbers.Add(barberTask);
        }

        Task freeBarber = await Task.WhenAny(workingBarbers);
        workingBarbers.Remove(freeBarber);
    }

    isBarberShopClosed = true;
    Console.WriteLine("The barberShop is closing for the day.");
}

async Task MoreClients()
{
    int value = 10;
    while (!isBarberShopClosed)
    {
        NewClient(value++);
        await Task.Delay(10000);
    }
}
record Client(string Name, int TimeInSeconds);