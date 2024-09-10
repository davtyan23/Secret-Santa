using SecretSantaTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//user list 
         List<Users> users;
 
        {
            users = new List<Users>();
            users.Add(new Users() { number = "09923659", name = "Liana", surname = "Manukyan" });
            users.Add(new Users() { number = "03311552", name = "Anna", surname = "Stepanyan" });
            users.Add(new Users() { number = "01546546", name = "Armen", surname = "Vardanyan" });
            users.Add(new Users() { number = "01354684", name = "Antur", surname = "Grigoryan" });
            users.Add(new Users() { number = "09544645", name = "Lenon", surname = "Amiryan" });
            users.Add(new Users() { number = "06476876", name = "Anna", surname = "Kirakosyan" });
            users.Add(new Users() { number = "03422155", name = "Vana", surname = "Ananayan" });
            users.Add(new Users() { number = "08900954", name = "Artak", surname = "Abovyan" });

    //print of all members
            foreach (var user in users)
            {
                Console.WriteLine(user.surname);
                Console.WriteLine(user.name);
                Console.WriteLine(user.number);
            }
            int members = 0;
            foreach (var user in users)
            {
                members++;
            }
            Console.WriteLine($"COUNT {members}");
        //matching users
            var randomSantaFullName = new Random();
            var randomRecieverFullName = new Random();
    
    for (int i = 0; i < members; i++)
    {
        int randomSantaIndex = randomSantaFullName.Next(users.Count);
        Users santa = users[randomSantaIndex];
        string PickedSantaName = users[randomSantaIndex].name;
        string PickedSantaSurname = users[randomSantaIndex].surname;
        string PickedSantaNumber = users[randomSantaIndex].number;
        Console.WriteLine($"Secret santa's Name|Surname|Number : {PickedSantaName} {PickedSantaSurname} {PickedSantaNumber}");
        int randomReceiverIndex = randomRecieverFullName.Next(users.Count);
        Users receiver = users[randomReceiverIndex];
        while (santa.number == receiver.number)
        {
            randomReceiverIndex = randomRecieverFullName.Next(users.Count);
            receiver = users[randomReceiverIndex];
        }

        //excepting users part should be located here
        users.RemoveAt(randomReceiverIndex);
        users.RemoveAt(randomSantaIndex);
    

        string pickedReceiverName = users[randomReceiverIndex].name;
        string pickedReceiverSurname = users[randomReceiverIndex].surname;
        string pickedReceiverNumber = users[randomReceiverIndex].number;
        Console.WriteLine($"Secrete receiver's Name|Surename|Number: {pickedReceiverName} {pickedReceiverSurname} {pickedReceiverNumber} ");
        
    }
}
    
  