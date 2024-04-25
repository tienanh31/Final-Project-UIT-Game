using System;

namespace RandomWalker
{
class Program
    {
   class Object{
        // Khởi tạo vị trí
        public int x { get; set; }
        public int y { get; set; }
        // Khởi tạo số lượng bước di chuyển
        public int numberStep { get; set;}
        // Khởi tạo mảng 2 chiều
        public int[,] Array2D { get; set; }
        public void EnterNumberStep (int numberStep)
        {
            this.numberStep=numberStep;
        }
        public void PrintArray2D()
        {
            for (int i = 0; i < numberStep; i++)
            {
                Console.WriteLine($"{Array2D[i, 0]}, {Array2D[i, 1]}");
            }
        }
        //Phương thức chính xử lý bước đi
         public void Walking()
        {
            Array2D = new int[numberStep, 2];
            Random random = new Random();
            for (int i = 0; i < numberStep; i++)
            {
                // Chọn đường đi ngẫu nhiên
                int direction = random.Next(4); 
                // 0: Trên, 1: Phải, 2: Dưới, 3: Trái
                switch (direction)
                {
                    case 0:
                        y++;
                        break;
                    case 1:
                        x++;
                        break;
                    case 2:
                        y--;
                        break;
                    case 3:
                        x--;
                        break;
                }
                Console.WriteLine($"Step {i + 1}: ({x}, {y})");
                // Lưu vị trí hiện tại vào mảng 2 chiều
                Array2D[i, 0] = x;
                Array2D[i, 1] = y;
              
            }
        }
  }
    
        static void Main(string[] args)
    {
       Object object1=new Object();
        int Steps;
        Console.Write("Enter number of step: ");
        while (!int.TryParse(Console.ReadLine(), out Steps))
        { 
            Console.WriteLine("Invalid, try again.");
            Console.Write("Enter number of step: ");
        }
       object1.EnterNumberStep(Steps);
       object1.Walking();
       object1.PrintArray2D();

       Console.ReadLine();
    }
    }
}