using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day07
{
    class Program
    {
        struct Position
        {
            public int x;
            public int y;

            public Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

        }

        enum Tilemap
        {
            blank,  //빈칸
            wall,   //벽 : 밀 수 없음
            box,    //박스 : 밀 수 있음
            coin,   //코인 : 먹을 수 있음
            mine   //지뢰 : 해체해야 함, 해제하지 않고 지뢰 구역으로 가면 피해를 입음

        }
        #region 맵 형태
        public static int[,,] mapBegin = new int[3, 15, 15]{
            //스테이지 1
            {{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1},
            { 1, 2, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1},
            { 1, 0, 0, 1, 4, 1, 1, 1, 1, 0, 0, 0, 0, 3, 1},
            { 1, 0, 2, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1},
            { 1, 0, 0, 1, 0, 1, 1, 1, 0, 2, 1, 1, 1, 1, 1},
            { 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
            { 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
            { 1, 0, 0, 1, 0, 1, 1, 1, 2, 2, 0, 1, 1, 1, 1},
            { 1, 4, 4, 1, 0, 1, 1, 1, 4, 4, 0, 1, 1, 1, 1},
            { 1, 3, 3, 1, 0, 1, 1, 1, 1, 1, 4, 0, 1, 1, 1},
            { 1, 0, 0, 1, 3, 1, 1, 1, 1, 1, 0, 3, 1, 1, 1},
            { 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}},
            //스테이지 2
            {{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 0, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 0, 1, 1, 1, 0, 4, 1, 1, 4, 1, 1, 1, 1},
            { 1, 2, 0, 1, 0, 1, 0, 4, 1, 1, 0, 1, 1, 1, 1},
            { 1, 0, 0, 1, 4, 1, 0, 4, 1, 0, 0, 0, 0, 1, 1},
            { 1, 0, 2, 1, 0, 1, 4, 1, 1, 0, 0, 1, 1, 1, 1},
            { 1, 0, 0, 1, 0, 1, 0, 1, 0, 2, 1, 1, 1, 1, 1},
            { 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
            { 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
            { 1, 0, 0, 1, 0, 1, 1, 1, 2, 2, 0, 1, 1, 1, 1},
            { 1, 4, 4, 1, 0, 1, 1, 1, 4, 4, 3, 1, 1, 1, 1},
            { 1, 4, 3, 1, 0, 1, 1, 1, 1, 1, 4, 0, 1, 1, 1},
            { 1, 4, 4, 1, 3, 1, 1, 1, 1, 1, 0, 3, 1, 1, 1},
            { 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}},
            // 스테이지 3
            {{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 0, 0, 4, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 3, 1, 1, 1, 1},
            { 1, 2, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1},
            { 1, 0, 0, 1, 4, 1, 0, 1, 1, 0, 4, 4, 0, 3, 1},
            { 1, 0, 2, 1, 0, 1, 0, 1, 1, 2, 0, 1, 1, 1, 1},
            { 1, 0, 0, 1, 0, 1, 0, 1, 2, 2, 1, 1, 1, 1, 1},
            { 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1},
            { 1, 0, 0, 2, 0, 0, 2, 0, 0, 0, 1, 1, 1, 1, 1},
            { 1, 0, 0, 1, 0, 0, 0, 0, 2, 2, 0, 1, 1, 1, 1},
            { 1, 4, 4, 1, 0, 2, 2, 2, 4, 4, 0, 1, 1, 1, 1},
            { 1, 4, 4, 1, 0, 2, 3, 0, 4, 1, 4, 0, 1, 1, 1},
            { 1, 0, 0, 1, 0, 0, 2, 0, 0, 1, 0, 3, 1, 1, 1},
            { 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}
        };
        #endregion

        #region 플레이어 위치
        public static int[,] playerBeginPos = new int[3, 2]
        {
            {1, 1},
            {1, 1},
            {1, 1}
        };

        #endregion
        static void Main(string[] args)
        {
            // 참고로 윈도우 터미널의 시작 크기는 열32 행7
            // 글꼴은 D2Coding, 글꼴 크기는 30, 줄 높이는 0.5로 설정 후 실행하였다.
            // 파일을 수정
            // 원격 저장소에서 파일을 수정
            Position playerPos = new Position();
            int[,] map;
            int stage = 1;
            int score = 0;
            int heart = 3;
            int defusingCount = 3;
            bool gameOver = false;
            bool clear = false;
            Start(stage, ref playerPos, out map);
            ConsoleKey key = ConsoleKey.W;

            while (!gameOver)
            {
                Render(playerPos, map, key, stage, score, heart, defusingCount);
                key = Input();
                Update(key, ref playerPos, map, ref score, ref heart, ref defusingCount, stage);
                // 맵에 있는 코인을 다 먹었는지 확인
                if (IsClear(map))
                {
                    // 다 먹었으면 점수 +50, 다음 스테이지로
                    score += 50;
                    defusingCount = 3;
                    stage++;
                    // 스테이지를 다 깼으면
                    if (stage > 3)
                    {
                        // clear true로 
                        clear = true;
                    }
                    //그게 아니라면 스테이지에 맞게 맵과 플레이어 위치 선정
                    else
                    {
                        playerPos = new Position(playerBeginPos[stage - 1, 0], playerBeginPos[stage - 1, 1]);
                        MakeMap(stage, mapBegin, out map);
                    }
                }
                // 목숨을 다 했거나 스테이지를 다 깻으면 게임 끝내기
                if (heart == 0 || clear)
                {
                    gameOver = true;
                }
            }
            if (stage > 3)
                stage = 3;
            End(clear, stage, score);
        }


        static void Start(int stage, ref Position playerPos, out int[,] map)
        {
            //Console.SetWindowSize(35, 20);
            Console.CursorVisible = false;
            playerPos = new Position(playerBeginPos[stage, 0], playerBeginPos[stage, 1]);
            MakeMap(stage, mapBegin, out map);
            showTitle();
        }

        // 타이틀 화면 출력
        static void showTitle()
        {
            Console.SetCursorPosition(5, 2);
            Console.WriteLine("----------------------");
            Console.SetCursorPosition(11, 4);
            Console.WriteLine("Coin 먹기");
            Console.SetCursorPosition(5, 6);
            Console.WriteLine("----------------------");
            Console.SetCursorPosition(8, 10);
            Console.WriteLine("아무키나 눌러서");
            Console.SetCursorPosition(8, 12);
            Console.WriteLine("시작하세요....");

            Console.ReadKey(true);
            Console.Clear();
        }

        //맵 생성
        static void MakeMap(int stage, int[,,] initialMap, out int[,] map)
        {
            map = new int[initialMap.GetLength(1), initialMap.GetLength(2)];
            for (int i = 0; i < initialMap.GetLength(1); i++)
            {
                for (int j = 0; j < initialMap.GetLength(2); j++)
                {
                    map[i, j] = initialMap[stage - 1, i, j];
                }

            }
        }

        // 출력 작업
        static void Render(Position playerPos, int[,] map, ConsoleKey key, int stage, int score, int heart, int defusingCount)
        {
            PrintMap(map);
            PrintPlayer(playerPos, key);
            PrintInfo(stage, score, heart, defusingCount);
        }

        // 플레이어 출력
        static void PrintPlayer(Position playerPos, ConsoleKey key)
        {
            Console.SetCursorPosition(playerPos.x, playerPos.y);
            Console.ForegroundColor = ConsoleColor.Cyan;
            switch (key)
            {
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    Console.WriteLine('◀');
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    Console.WriteLine('▶');
                    break;
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    Console.WriteLine('▲');
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                default:
                    Console.WriteLine('▼');
                    break;
            }
            Console.ResetColor();
        }

        // 맵 출력
        static void PrintMap(int[,] map)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Console.SetCursorPosition(x, y);
                    switch (map[y, x])
                    {
                        case (int)Tilemap.blank:
                            Console.Write(' ');
                            break;
                        case (int)Tilemap.wall:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write('▦');
                            break;
                        case (int)Tilemap.box:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write('□');
                            break;
                        case (int)Tilemap.coin:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write('◎');
                            break;
                        case (int)Tilemap.mine:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write('△');
                            break;
                    }
                    Console.ResetColor();
                }

            }
        }

        // 정보 출력
        static void PrintInfo(int stage, int score, int heart, int defusingCount)
        {
            Console.SetCursorPosition(mapBegin.GetLength(1) + 2, 1);
            Console.Write("스테이지 : {0}", stage);
            Console.SetCursorPosition(mapBegin.GetLength(1) + 2, 3);
            Console.Write("점수 :         ");
            Console.SetCursorPosition(mapBegin.GetLength(1) + 10, 3);
            Console.Write(score);
            Console.SetCursorPosition(mapBegin.GetLength(1) + 2, 5);
            Console.Write("목숨 : ");
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < 3; i++)
            {
                if (i < heart)
                    Console.Write("♥ ");
                else
                    Console.Write("♡ ");
            }
            Console.ResetColor();
            Console.SetCursorPosition(mapBegin.GetLength(1) + 2, 7);
            Console.Write("해체 : ");
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < 3; i++)
            {
                if (i < defusingCount)
                    Console.Write("♠ ");
                else
                    Console.Write("♤ ");
            }
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(mapBegin.GetLength(1) + 2, 9);
            Console.Write("WASD : 이동");
            Console.SetCursorPosition(mapBegin.GetLength(1) + 2, 11);
            Console.Write("R : 리셋");
            Console.SetCursorPosition(mapBegin.GetLength(1) + 2, 13);
            Console.Write("F : 해체");
            Console.ResetColor();
        }

        // 입력 작업
        static ConsoleKey Input()
        {
            return Console.ReadKey(true).Key;
        }

        // 최신화 작업
        static void Update(ConsoleKey key, ref Position playerPos, int[,] map, ref int score, ref int heart, ref int defusingCount, int stage)
        {
            // 움직임
            Move(key, ref playerPos, map, ref score, ref heart, stage);
            SpecialAction(key, ref playerPos, map, ref score, ref heart, ref defusingCount, stage);
        }

        // 입력에 따른 움직임 구현
        static void Move(ConsoleKey key, ref Position playerPos, int[,] map, ref int score, ref int heart, int stage)
        {
            Position targetPos = new Position(playerPos.x, playerPos.y);
            Position overPos = new Position(playerPos.x, playerPos.y);

            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    targetPos = new Position(playerPos.x, playerPos.y - 1);
                    overPos = new Position(playerPos.x, playerPos.y - 2);
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    targetPos = new Position(playerPos.x - 1, playerPos.y);
                    overPos = new Position(playerPos.x - 2, playerPos.y);
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    targetPos = new Position(playerPos.x, playerPos.y + 1);
                    overPos = new Position(playerPos.x, playerPos.y + 2);
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    targetPos = new Position(playerPos.x + 1, playerPos.y);
                    overPos = new Position(playerPos.x + 2, playerPos.y);
                    break;
                default:
                    return;
            }
            // 타일 맵이 빈칸이면
            if (map[targetPos.y, targetPos.x] == (int)Tilemap.blank)
            {
                playerPos = new Position(targetPos.x, targetPos.y);
            }
            else if (map[targetPos.y, targetPos.x] == (int)Tilemap.box)
            {
                if (map[overPos.y, overPos.x] == (int)Tilemap.blank)
                {
                    map[overPos.y, overPos.x] = (int)Tilemap.box;
                    map[targetPos.y, targetPos.x] = (int)Tilemap.blank;
                    playerPos = new Position(targetPos.x, targetPos.y);
                }
                else if (map[overPos.y, overPos.x] == (int)Tilemap.mine)
                {
                    map[overPos.y, overPos.x] = (int)Tilemap.blank;
                    map[targetPos.y, targetPos.x] = (int)Tilemap.blank;
                    playerPos = new Position(targetPos.x, targetPos.y);
                }
            }
            else if (map[targetPos.y, targetPos.x] == (int)Tilemap.coin)
            {
                map[targetPos.y, targetPos.x] = (int)Tilemap.blank;
                score += 10;
                playerPos = new Position(targetPos.x, targetPos.y);
            }
            else if (map[targetPos.y, targetPos.x] == (int)Tilemap.mine)
            {
                map[targetPos.y, targetPos.x] = (int)Tilemap.blank;
                heart--;
                playerPos = new Position(targetPos.x, targetPos.y);
            }
        }

        // 입력에 따른 특수 행동 구현
        static void SpecialAction(ConsoleKey key, ref Position playerPos, int[,] map, ref int score, ref int heart, ref int defusingCount, int stage)
        {
            switch (key)
            {
                case ConsoleKey.F:
                    if (defusingCount > 0)
                    {
                        defusingCount--;
                        DefusingMine(playerPos, map);
                    }
                    break;
                case ConsoleKey.R:
                    heart = 3;
                    defusingCount = 3;
                    Restart(ref score, stage, ref playerPos, map);
                    break;
                default:
                    return;
            }
        }

        // 리셋 키를 눌렀을 때 실행
        static void Restart(ref int score, int stage, ref Position playerPos, int[,] map)
        {
            score -= 50;
            playerPos = new Position(playerBeginPos[stage - 1, 0], playerBeginPos[stage - 1, 1]);
            MakeMap(stage, mapBegin, out map);
        }

        // 해체 키를 눌렀을 때 실행
        static void DefusingMine(Position playerPos, int[,] map)
        {
            Position wPos = new Position(playerPos.x, playerPos.y - 1);
            Position aPos = new Position(playerPos.x - 1, playerPos.y);
            Position sPos = new Position(playerPos.x, playerPos.y + 1);
            Position dPos = new Position(playerPos.x + 1, playerPos.y);

            if (map[wPos.y, wPos.x] == (int)Tilemap.mine)
            {
                map[wPos.y, wPos.x] = (int)Tilemap.blank;
            }
            if (map[aPos.y, aPos.x] == (int)Tilemap.mine)
            {
                map[aPos.y, aPos.x] = (int)Tilemap.blank;
            }
            if (map[sPos.y, sPos.x] == (int)Tilemap.mine)
            {
                map[sPos.y, sPos.x] = (int)Tilemap.blank;
            }
            if (map[dPos.y, dPos.x] == (int)Tilemap.mine)
            {
                map[dPos.y, dPos.x] = (int)Tilemap.blank;
            }
        }

        // 맵에 코인을 다 먹었는지 확인 후 있으면 false, 없으면 true 반환
        static bool IsClear(int[,] map)
        {
            foreach (var s in map)
            {
                if (s == (int)Tilemap.coin)
                    return false;
            }
            return true;
        }

        // 끝났을 때 출력할 함수
        static void End(bool clear, int stage, int score)
        {
            Console.Clear();
            if (clear)
            {
                Console.SetCursorPosition(12, 1);
                Console.WriteLine("Game Clear");
            }
            else
            {
                Console.SetCursorPosition(12, 1);
                Console.WriteLine("Game Over");
            }
            Console.SetCursorPosition(5, 3);
            Console.WriteLine("----------------------");
            Console.SetCursorPosition(10, 5);
            Console.WriteLine("스테이지 : {0}", stage);
            Console.SetCursorPosition(13, 7);
            Console.WriteLine("점수 : {0}", score);
            Console.SetCursorPosition(5, 9);
            Console.WriteLine("----------------------");
            Console.SetCursorPosition(8, 11);
            Console.WriteLine("아무키나 누르면");
            Console.SetCursorPosition(8, 13);
            Console.WriteLine("종료됩니다....");
            Console.ReadKey(true);
            Console.Clear();
        }
    }
}
