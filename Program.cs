using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Tugaspbo
{
    public interface IKemampuan
    {
        string Nama { get; }
        int Cooldown { get; }
        int CooldownAktif { get; set; }
        void Gunakan(Robot target);
    }

    public abstract class Robot
    {
        public string Nama { get; set; }
        public int Energi { get; set; }
        public int Armor { get; set; }
        public int Serangan { get; set; }
        private int _maxEnergi;

        public Robot(string nama, int energi, int armor, int serangan)
        {
            Nama = nama;
            Energi = energi;
            Armor = armor;
            Serangan = serangan;
            _maxEnergi = energi;
        }

        public abstract void Serang(Robot target);

        public virtual void CetakInformasi()
        {
            Console.WriteLine($"{Nama} | Energi: {Energi}, Armor: {Armor}, Serangan: {Serangan}");
        }

        public void GunakanKemampuan(IKemampuan kemampuan, Robot target)
        {
            if (kemampuan.CooldownAktif == 0)
            {
                kemampuan.Gunakan(target);
                kemampuan.CooldownAktif = kemampuan.Cooldown;
            }
            else
            {
                Console.WriteLine($"{kemampuan.Nama} sedang cooldown! {kemampuan.CooldownAktif} giliran tersisa.");
            }
        }

        public void PulihkanEnergi()
        {
            Energi = _maxEnergi;
            Console.WriteLine($"{Nama} memulihkan energi.");
        }
    }

    public class BosRobot : Robot
    {
        public BosRobot(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
        {
        }

        public override void Serang(Robot target)
        {
            Console.WriteLine($"{Nama} menyerang {target.Nama} dengan kekuatan serangan {Serangan}.");
            target.Energi -= Serangan - target.Armor;
            if (target.Energi < 0) target.Energi = 0;

            target.Armor -= 5;
            if (target.Armor < 0) target.Armor = 0;
        }

        public void Diserang(Robot penyerang)
        {
            Console.WriteLine($"{Nama} diserang oleh {penyerang.Nama}.");
            Energi -= penyerang.Serangan - Armor;
            if (Energi < 0) Energi = 0;
        }

        public void Mati()
        {
            if (Energi <= 0)
            {
                Console.WriteLine($"{Nama} telah dikalahkan!");
            }
        }
    }

    public class Perbaikan : IKemampuan
    {
        public string Nama => "Perbaikan";
        public int Cooldown { get; private set; }
        public int CooldownAktif { get; set; }

        public Perbaikan()
        {
            Cooldown = 3;
            CooldownAktif = 0;
        }

        public void Gunakan(Robot target)
        {
            Console.WriteLine($"{target.Nama} menggunakan {Nama} untuk memulihkan energi.");
            target.Energi += 40;
            if (target.Energi > 100) target.Energi = 100;
        }
    }

    public class SeranganListrik : IKemampuan
    {
        public string Nama => "Serangan Listrik";
        public int Cooldown { get; private set; }
        public int CooldownAktif { get; set; }

        public SeranganListrik()
        {
            Cooldown = 2;
            CooldownAktif = 0;
        }

        public void Gunakan(Robot target)
        {
            Console.WriteLine($"{target.Nama} terkena {Nama}, gerakan melambat!");
            target.Energi -= 15;
        }
    }

    public class SeranganPlasma : IKemampuan
    {
        public string Nama => "Serangan Plasma";
        public int Cooldown { get; private set; }
        public int CooldownAktif { get; set; }

        public SeranganPlasma()
        {
            Cooldown = 4;
            CooldownAktif = 0;
        }

        public void Gunakan(Robot target)
        {
            Console.WriteLine($"{target.Nama} terkena {Nama}!");
            target.Energi -= 30;
        }
    }

    public class PertahananSuper : IKemampuan
    {
        public string Nama => "Pertahanan Super";
        public int Cooldown { get; private set; }
        public int CooldownAktif { get; set; }

        public PertahananSuper()
        {
            Cooldown = 5;
            CooldownAktif = 0;
        }

        public void Gunakan(Robot target)
        {
            Console.WriteLine($"{target.Nama} meningkatkan pertahanan dengan {Nama}.");
            target.Armor += 20;
        }
    }

    public class Buff : IKemampuan
    {
        public string Nama => "Buff";
        public int Cooldown { get; private set; }
        public int CooldownAktif { get; set; }

        public Buff()
        {
            Cooldown = 4;
            CooldownAktif = 0;
        }

        public void Gunakan(Robot target)
        {
            Console.WriteLine($"{target.Nama} menggunakan {Nama}! Serangan meningkat 3 kali lipat untuk satu giliran.");
            target.Serangan *= 3;
        }
    }

    public class RobotTempur : Robot
    {
        public RobotTempur(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
        {
        }

        public override void Serang(Robot target)
        {
            Console.WriteLine($"{Nama} menyerang {target.Nama}.");
            target.Energi -= Serangan - target.Armor;
            if (target.Energi < 0) target.Energi = 0;
        }
    }

    public class RobotPlasma : Robot
    {
        public RobotPlasma(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
        {
        }

        public override void Serang(Robot target)
        {
            Console.WriteLine($"{Nama} menyerang {target.Nama} dengan serangan plasma!");
            target.Energi -= Serangan;
            if (target.Energi < 0) target.Energi = 0;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Robot robot1 = new RobotTempur("Robot Tempur GWEH", 150, 10, 20);
            BosRobot bos = new BosRobot("Bos Robot AJAW", 200, 15, 30);

            IKemampuan repair = new Perbaikan();
            IKemampuan electricShock = new SeranganListrik();
            IKemampuan plasmaCannon = new SeranganPlasma();
            IKemampuan superShield = new PertahananSuper();
            IKemampuan buff = new Buff();

            Console.WriteLine("Status Robot Pemain:");
            robot1.CetakInformasi();

            Console.WriteLine("\nStatus Bos Robot:");
            bos.CetakInformasi();

            while (bos.Energi > 0)
            {
                Console.WriteLine("\nPilih jurus untuk digunakan:");
                Console.WriteLine("1. Serangan Biasa");
                Console.WriteLine("2. Serangan Plasma (Cooldown: {0})", plasmaCannon.CooldownAktif);
                Console.WriteLine("3. Perbaikan (Cooldown: {0})", repair.CooldownAktif);
                Console.WriteLine("4. Serangan Listrik (Cooldown: {0})", electricShock.CooldownAktif);
                Console.WriteLine("5. Pertahanan Super (Cooldown: {0})", superShield.CooldownAktif);
                Console.WriteLine("6. Buff (Cooldown: {0})", buff.CooldownAktif);

                string pilihan = Console.ReadLine();

                switch (pilihan)
                {
                    case "1":
                        robot1.Serang(bos);
                        break;
                    case "2":
                        robot1.GunakanKemampuan(plasmaCannon, bos);
                        break;
                    case "3":
                        robot1.GunakanKemampuan(repair, robot1);
                        break;
                    case "4":
                        robot1.GunakanKemampuan(electricShock, bos);
                        break;
                    case "5":
                        robot1.GunakanKemampuan(superShield, robot1);
                        break;
                    case "6":
                        robot1.GunakanKemampuan(buff, robot1);
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid.");
                        break;
                }


                if (bos.Energi <= 0)
                {
                    bos.Mati();
                    break;
                }

                bos.Serang(robot1);
                robot1.CetakInformasi();

                if (robot1.Energi <= 0)
                {
                    Console.WriteLine($"{robot1.Nama} telah kalah!");
                    break;
                }

                if (plasmaCannon.CooldownAktif > 0) plasmaCannon.CooldownAktif--;
                if (repair.CooldownAktif > 0) repair.CooldownAktif--;
                if (electricShock.CooldownAktif > 0) electricShock.CooldownAktif--;
                if (superShield.CooldownAktif > 0) superShield.CooldownAktif--;
                if (buff.CooldownAktif > 0) buff.CooldownAktif--;

                Console.Clear();

                Console.WriteLine("\nStatus Robot Pemain:");
                robot1.CetakInformasi();

                Console.WriteLine("\nStatus Bos Robot:");
                bos.CetakInformasi();
            }
        }
    }
}