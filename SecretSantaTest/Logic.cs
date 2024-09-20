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



List<Users> receivers = new List<Users>(users);
Random random = new Random();

receivers = receivers.OrderBy(x => random.Next()).ToList();
for (int i = 0; i < users.Count; i++)
{
    if (users[i] == receivers[i])
    {
        receivers = receivers.OrderBy(x => random.Next()).ToList();
        i = 0;
    }
}
Dictionary<Users, Users> santaPairs = new Dictionary<Users, Users>();

for (int i = 0; i < users.Count; i++)
{
    santaPairs[users[i]] = receivers[i];
}
foreach (var pair in santaPairs)
{
    Console.WriteLine($"Santa: {pair.Key.name} {pair.Key.surname} Receiver: {pair.Value.name} {pair.Value.surname}");
}



