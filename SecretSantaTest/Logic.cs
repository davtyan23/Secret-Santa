using System;
using System.Collections.Generic;
using System.Linq;

public class Users
{
    public string name;
    public string surname;
    public string number;

    public override string ToString()
    {
        return $"{name} {surname}";
    }
}

class SecretSanta
{
    static void Main(string[] args)
    {
        // Main users list
        List<Users> users = new List<Users>()
        {
            new Users() { number = "09923659", name = "Liana", surname = "Manukyan" },
            new Users() { number = "03311552", name = "Anna", surname = "Stepanyan" },
            new Users() { number = "01546546", name = "Armen", surname = "Vardanyan" },
            new Users() { number = "01354684", name = "Antur", surname = "Grigoryan" },
            new Users() { number = "09544645", name = "Lenon", surname = "Amiryan" },
            new Users() { number = "06476876", name = "Anna", surname = "Kirakosyan" },
            new Users() { number = "03422155", name = "Vana", surname = "Ananayan" },
            new Users() { number = "08900954", name = "Artak", surname = "Abovyan" }
        };

        List<Users> santa = new List<Users>();
        List<Users> receiver = new List<Users>();

        Random random = new Random();

        // Main loop to create the pairs
        for (int i = users.Count; i > 0; i--)
        {
            // Getting available users for both Santa and receiver
            var notSantaUser = users.Where(x => !santa.Contains(x)).ToList();
            var notReceiverUser = users.Where(x => !receiver.Contains(x)).ToList();

            // Randomly selecting Santa and Receiver
            var randomSantaIndex = random.Next(notSantaUser.Count);
            var randomReceiverIndex = random.Next(notReceiverUser.Count);

            // Ensure Santa and Receiver are not the same
            while (notSantaUser[randomSantaIndex] == notReceiverUser[randomReceiverIndex])
            {
                randomReceiverIndex = random.Next(notReceiverUser.Count);
            }

            // Add selected users to lists
            santa.Add(notSantaUser[randomSantaIndex]);
            receiver.Add(notReceiverUser[randomReceiverIndex]);

            // Display the pairing
            Console.WriteLine("Santa Name :" + notSantaUser[randomSantaIndex].name + " " + notSantaUser[randomSantaIndex].surname +
                            " Receiver Name: " + notReceiverUser[randomReceiverIndex].name + " " + notReceiverUser[randomReceiverIndex].surname);
        }

        // Pairing dictionary
        Dictionary<Users, Users> aih = new Dictionary<Users, Users>();
        for (int k = 0; k < users.Count; k++)
        {
            aih[santa[k]] = receiver[k];
        }

        // Display the dictionary
        foreach (var pair in aih)
        {
            Console.WriteLine($"{pair.Key} -> {pair.Value}");
        }
    }
}
