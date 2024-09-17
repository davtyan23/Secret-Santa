using SecretSantaTest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//user list 

//dictionary
Users user1 = new Users() { name = "Liana", surname = "Manukyan", number = "09923659" };
Users user15 = new Users() { name = "Liana", surname = "Manukyan", number = "09923659" };
Users user2 = new Users() { name = "Anna", surname = "Stepanyan", number = "03311552" };
Users user3 = new Users() { name = "Armen", surname = "Vardanyan", number = "01546546" };
Users user4 = new Users() { number = "01354684", name = "Antur", surname = "Grigoryan" };
Users user5 = new Users() { number = "09544645", name = "Lenon", surname = "Amiryan" };
Users user6 = new Users() { number = "06476876", name = "Anna", surname = "Kirakosyan" };
Users user7 = new Users() { number = "03422155", name = "Vana", surname = "Ananayan" };
Users user8 = new Users() { number = "08900954", name = "Artak", surname = "Abovyan" };

//Dictionary<Users, Users> aih = new Dictionary<Users, Users>()
//{
//    {user1,user1},
//    {user15,user1}

//};
//var a = aih.GetValueOrDefault(user1).name;
//var b = aih.ContainsKey(user1);

//aih.Remove(user1);
//var c = aih.ContainsKey(user1);
//aih.Add(user1, user2);
//var d = aih.ElementAt(0);
//var g = aih[user1];
//dictionary

//MAIN USERS list
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

}
//MAIN USERS list

//list SANTA
List<Users> santa;

{
    santa = new List<Users>();
    //santa.Add(new Users() { number = "09923659", name = "Liana", surname = "Manukyan" });
    //santa.Add(new Users() { number = "03311552", name = "Anna", surname = "Stepanyan" });
    //santa.Add(new Users() { number = "01546546", name = "Armen", surname = "Vardanyan" });
    //santa.Add(new Users() { number = "01354684", name = "Antur", surname = "Grigoryan" });
    //santa.Add(new Users() { number = "09544645", name = "Lenon", surname = "Amiryan" });
    //santa.Add(new Users() { number = "06476876", name = "Anna", surname = "Kirakosyan" });
    //santa.Add(new Users() { number = "03422155", name = "Vana", surname = "Ananayan" });
    ////////santa.Add(new Users() { number = "08900954", name = "Artak", surname = "Abovyan" });

}
//list SANTA

//receiver list
List<Users> receiver;

{
    receiver = new List<Users>();
    //receiver.Add(new Users() { number = "09923659", name = "Liana", surname = "Manukyan" });
    //receiver.Add(new Users() { number = "03311552", name = "Anna", surname = "Stepanyan" });
    //receiver.Add(new Users() { number = "01546546", name = "Armen", surname = "Vardanyan" });
    //receiver.Add(new Users() { number = "01354684", name = "Antur", surname = "Grigoryan" });
    //receiver.Add(new Users() { number = "09544645", name = "Lenon", surname = "Amiryan" });
    //receiver.Add(new Users() { number = "06476876", name = "Anna", surname = "Kirakosyan" });
    //receiver.Add(new Users() { number = "03422155", name = "Vana", surname = "Ananayan" });
    //receiver.Add(new Users() { number = "08900954", name = "Artak", surname = "Abovyan" });

}


//receiver list end

Random random = new Random();
//int santaCount = santa.Count;

    for (int i = users.Count; i > 0; i--)
    {
        //getting available users for both, santa and reciver
        var notSantaUser = users.Where(x => !santa.Contains(x)).ToList();
        var notReciverUser = users.Where(x => !receiver.Contains(x)).ToList();

        var randomSantaIndex = random.Next(notSantaUser.Count);
        var randomReciverIndex = random.Next(notReciverUser.Count);

        //check that santa != receiver
        bool checkNotSame = false;

    while (randomSantaIndex == randomReciverIndex)
    {
        randomSantaIndex = random.Next(notSantaUser.Count);
        randomReciverIndex = random.Next(notReciverUser.Count);
        if (notSantaUser.Count >= 0 ) 
        {
            i = users.Count;
            santa.Clear();
            receiver.Clear();
            break;
        }
    }
    //for (int j = 0; j < users.Count; j++)
    //{
    //    randomReciverIndex = random.Next(notReciverUser.Count);

    //    if (notSantaUser[randomSantaIndex].number != notReciverUser[randomReciverIndex].number)
    //    {
    //        checkNotSame = true;
    //        break;
    //    }
    //}
    //check end


    //1;2
    santa.Add(notSantaUser[randomSantaIndex]);
        receiver.Add(notReciverUser[randomReciverIndex]);

        Console.WriteLine("Santa Name :" + notSantaUser[randomSantaIndex].name + " " + notSantaUser[randomSantaIndex].surname +
                        " reciver Name: " + notReciverUser[randomReciverIndex].name + " " + notReciverUser[randomReciverIndex].surname);



    }
//pairing dictionary
Dictionary<Users, Users> aih = new Dictionary<Users, Users>();
   for (int k = 0; k < users.Count; k++)
        {
        aih[santa[k]] = receiver[k];
        }
   
//pairing dictionary

foreach (var pair in aih)
{
    Console.WriteLine($"{pair.Key} -> {pair.Value}");
}



