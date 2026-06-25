//ANTIGUO

using System.Diagnostics;
using System.Runtime.CompilerServices;
//const string Rank32 = "_S(0.015625f * _f(RANK + 1024))";

const bool Debugging = false;

string lifeInt = "";
string timeInt = "";
string deathFunc = "";
string lifeFunc = "";

string[] eosdSTG = new string[0];

string currentFunction = "";

string dialogInterrupt = ""; //beings empty and later contains the function to run after dialog
List<string> fwSTG = new List<string>();

List<string> enemyFunctions = new List<string>(); //used to store the names of the functions that require saving the registers into the enemy's variables

List<string> laserFunctions = new List<string>(); //used to know in which functions it should store the lasers' initial and current rotations

List<string> FuncsFound = new List<string>(); //used to store the stage's functions

List<string> bossEndFunctions = new List<string>(); //Used to store the functions used by bosses when they are defeated. (Mainly to tell the timeline when to resume)

//unused:
//List<MultiCallback> callbacks = new List<MultiCallback>(); //Used to store the functions to run on timeouts and when life reaches certain value

Dictionary<string, string> insNumbers = new Dictionary<string, string>()
{
    ["bullet_fan_aimed"] = "0",
    ["bullet_fan"] = "1",
    ["bullet_circle_aimed"] = "2",
    ["bullet_circle"] = "3",
    ["bullet_offset_circle_aimed"] = "4",
    ["bullet_offset_circle"] = "5",
    ["bullet_random_angle"] = "6",
    ["bullet_random_speed"] = "7",
    ["bullet_random"] = "8",
    ["jump_lss"] = "== -1)",
    ["jump_leq"] = "< 1)",
    ["jump_equ"] = "== 0)",
    ["jump_gre"] = "== 1)",
    ["jump_geq"] = "> -1)",
    ["jump_neq"] = "!= 0)",
    ["call_lss"] = "== -1)",
    ["call_leq"] = "< 1)",
    ["call_equ"] = "== 0)",
    ["call_gre"] = "== 1)",
    ["call_geq"] = "> -1)",
    ["call_neq"] = "!= 0)",
};

string temp = "";

bool exitInsideLoop = false; //used to exit certain loops

int tmpInt = 0;

int dummyInt = 0;

bool alreadySetFuncs = false; //Used to tell if the variables for the life and timeout interrupts are set

bool tempLaserMoves = false; //Tells if the laser that is going to spawn can move or if it is infinite

bool alreadySetLaser = false; //used to know if the laser index was set

bool alreadySetBackup = false; //used to know if the backup registers were set

bool alreadySetComp = false; //used to know if the comparison temp variable was set

bool alreadySetEX = false; //used to know if the bullet_effects variables (a, b, r, s) were already initialized inside the current function

bool alreadySetSound = false; //by default bullets that have the 512 flag (sound) will use the main sfx, but if it was set to custom it won't use the defaults

int timelinePos = 0; //EoSD timeline position

int fwInsert = 0; //Location to insert more code right before the timeline

bool autoShoot = true; //tells if it should automatically shoot bullets after setting their properties

bool outsideTimeline = true; //when 'true' it inserts lines instead of adding them. used to fill data between the start of the script and the timeline

Principal();


void Principal()
{

    Console.WriteLine("Write the number of the stage to convert (1-7)");
    string st = Console.ReadLine();

    if (st != null)
    {
        uint stg = 0;
        if (uint.TryParse(st, out stg))
        {
            if (stg > 0 && stg <= 7)
            {
                Convert((int)stg);
            }
            /*
            switch (stg)
            {
                case 1:
                    Stage1();
                    break;


                case 2:
                    Stage2();
                    break;


                case 3:
                    Stage3();
                    break;


                case 4:
                    Stage4();
                    break;


                case 5:
                    Stage5();
                    break;


                case 6:
                    Stage6();
                    break;


                case 7:
                    StageEX();
                    break;

            }*/
        }
        Console.WriteLine("Invalid Stage");
        Environment.Exit(-1);
    }
    Console.WriteLine("Invalid Stage");
    Environment.Exit(-1);
}

/*void Stage1()
{
    Convert(1)
}*/

//stages 1-7 (7 for extra stage)
void Convert(int stage)
{
    eosdSTG = File.ReadAllLines($"ecldata{stage}.txt");
    fwSTG.Clear();
    enemyFunctions.Clear();
    bossEndFunctions.Clear();

    outsideTimeline = true;
    a("anim { \"enemy.anm\"; \"st01enm.anm\"; }"); //Include ANM
    a("ecli { \"default.ecl\"; }"); //Include ECL
    a("global SCREEN_FIX = -192.0f;"); //Used to convert EoSD's X/Horizontal coordinates
    a("void MainBossSpell();");
    a("");
    a("void AutoDelay(int delay)");
    a("{");
    a("MainLoop:");
    //b(1, "ecl_time_sub((RAND_INT % delay) + (delay / 5));");
    //b(1, "ecl_time_sub(delay / 5);");
    b(1, "ecl_time_sub((RAND_INT % (delay / 2)) + (delay / 2));");
    b(1, "shoot_now(0);");
    b(1, "goto MainLoop;");
    a("}");
    a("");
    a("void Auto(int delay)");
    a("{");
    a("MainLoop:");
    b(1, "ecl_time_sub(delay);");
    b(1, "shoot_now(0);");
    b(1, "goto MainLoop;");
    a("}");
    a("");
    a("void SpellEnd()");
    a("{");
    b(1, "bullet_cancel_radius(640.0f);");
    b(1, "phase_timer_clear();");
    b(1, "async_stop_all();");
    b(1, "enemy_kill_all_stones();");
    b(1, "spellcard_end();");
    b(1, "GF0 = 0.0f;");
    b(1, "GF1 = 0.0f;");
    b(1, "GF2 = 0.2f;");
    b(1, "GF3 = 0.2f;");
    b(1, "GF4 = 0.2f;");
    b(1, "GF5 = 0.2f;");
    b(1, "ex_ins_repeat(0);");
    b(1, "laser_clear_all();");
    b(1, "effect_sound(27);");
    b(1, "player_protect_range(0.0f);");
    b(1, "move_velocity_abs(0.0f, 0.0f);");
    b(1, "move_velocity_abs_interp(0, 0, 0.0f, 0.0f);");
    b(1, "move_position_abs_interp(0, 0, 0.0f, 0.0f);");
    b(1, "$PLAYER_DEATHS = 0;");
    b(1, "$PLAYER_BOMBS_USED = 0;");
    b(1, "$SPELL_CAPTURE = 1;");
    a("}");
    a("");

    a("void BossDead()");
    a("{");
    b(1, "__enemy_manager_set_unknown_F(0);");
    b(1, "enemy_flags_set(156);");
    b(1, "effect_sound(5);");
    b(1, "move_velocity_abs(%RAND_ANGLE, 0.4f);");
    b(1, "enemy_create_rel(\"Ecl_EtBreak2_ni\", 0.0f, 0.0f, 9999, 0, 0);");
    b(1, "enemy_kill_all();");
    a("+60:");
    b(1, "bullet_cancel();");
    b(1, "spellcard_end();");
    b(1, "effect_screen_shake(30, 12, 0);");
    b(1, "anm_create_front(1, 25);");
    b(1, "anm_create_front(1, 57);");
    b(1, "effect_sound(5);");
    b(1, "boss_set(-1);");
    b(1, "enemy_delete();");
    b(1, "return;");
    a("}");
    a("");

    /*
    a("void RankSimulator()");
    a("{");
    b(1, "enemy_flags_set(32);");
    b(1, "GI2 = 16;");
    b(1, "int subRank = 0;");
    b(1, "EI0 = 0;"); //subrank
    a("!E");
    b(1, "12;");
    a("!NHL");
    b(1, "10;");
    a("!X");
    b(1, "14;");
    a("!*");
    b(1, "EI1 = [-1];"); //Minimum rank
    b(1, "");
    a("");
    a("!E");
    b(1, "20;");
    a("!NHL");
    b(1, "32;");
    a("!X");
    b(1, "18;");
    a("!*");
    b(1, "EI2 = [-1];"); //Maximum rank
    b(1, "@RankDeaths() async;");
    b(1, "@RankPower() async;");
    b(1, "@RankBombs() async;");
    b(1, "@RankPersist() async;");
    b(1, "ecl_time_sub(999999);");
    a("}");
    a("");

    a("");

    a("void RankPersist()");
    a("{");
    a("MainLoop:");
    b(1, "msg_wait();");
    b(1, "ecl_time_sub(1920);");
    b(1, "GI2 += 1;");
    b(1, "if (GI2 > EI2)"); //EI1 = Min. EI2 = Max
    b(1, "{");
    b(2, "GI2 = EI2;");
    b(1, "}");
    b(1, "goto MainLoop;");
    a("}");

    a("");

    a("void RankDeaths()");
    a("{");
    b(1, "int lastDeath = PLAYER_DEATHS;");
    a("MainLoop:");
    b(1, "if (PLAYER_DEATHS > lastDeath)");
    b(1, "{");
    b(2, "lastDeath = PLAYER_DEATHS;");
    b(2, "EI0 -= 1600;");
    a("MiniLoop:");
    b(2, "if (EI0 < 100)");
    b(2, "{");
    b(3, "GI2 -= 1;");
    b(3, "EI0 += 100;");
    b(3, "goto MiniLoop;");
    b(2, "}");
    b(2, "if (GI2 < EI1)"); //EI1 = Min. EI2 = Max
    b(2, "{");
    b(3, "GI2 = EI1;");
    b(2, "}");
    b(2, "else if (PLAYER_DEATHS < lastDeath)");
    b(2, "{");
    b(3, "lastDeath = PLAYER_DEATHS;");
    b(2, "}");
    b(1, "}");
    b(1, "ecl_time_sub(1);");
    b(1, "goto MainLoop;");
    a("}");
    a("");


    a("void RankPower()");
    a("{");
    b(1, "int lastPower = PLAYER_POWER;");
    a("MainLoop:");
    b(1, "if (PLAYER_POWER > lastPower)");
    b(1, "{");
    b(2, "lastPower = PLAYER_POWER;");
    b(2, "EI0 += 1;");
    a("MiniLoop:");
    b(2, "if (EI0 >= 100)");
    b(2, "{");
    b(3, "GI2 += 1;");
    b(3, "EI0 -= 100;");
    b(3, "goto MiniLoop;");
    b(2, "}");
    b(2, "if (GI2 > EI2)"); //EI1 = Min. EI2 = Max
    b(2, "{");
    b(3, "GI2 = EI2;");
    b(2, "}");
    b(2, "else if (PLAYER_POWER < lastPower)");
    b(2, "{");
    b(3, "lastPower = PLAYER_POWER;");
    b(2, "}");
    b(1, "}");
    b(1, "ecl_time_sub(1);");
    b(1, "goto MainLoop;");
    a("}");
    a("");



    a("void RankBombs()");
    a("{");
    b(1, "int lastBomb = PLAYER_BOMBS_USED;");
    a("MainLoop:");
    b(1, "if (PLAYER_BOMBS_USED > lastBomb)");
    b(1, "{");
    b(2, "lastBomb = PLAYER_BOMBS_USED;");
    b(2, "EI0 -= 200;");
    a("MiniLoop:");
    b(2, "if (EI0 <= 100)");
    b(2, "{");
    b(3, "GI2 -= 1;");
    b(3, "EI0 += 100;");
    b(3, "goto MiniLoop;");
    b(2, "}");
    b(2, "if (GI2 < EI1)"); //EI1 = Min. EI2 = Max
    b(2, "{");
    b(3, "GI2 = EI1;");
    b(2, "}");
    b(2, "else if (PLAYER_BOMBS_USED < lastBomb)");
    b(2, "{");
    b(3, "lastBomb = PLAYER_BOMBS_USED;");
    b(2, "}");
    b(1, "}");
    b(1, "ecl_time_sub(1);");
    b(1, "goto MainLoop;");
    a("}");
    a("");
    */


    fixInsVars(); //changes variables to match FW's

    outsideTimeline = false;
    for (int i = eosdSTG.Length - 1; i > 0; i--) //Search for the timeline
    {
        if (eosdSTG[i].StartsWith("timeline "))
        {
            timelinePos = i;
            break;
        }
    }


    a("void main()");
    a("{");
    a("    enemy_flags_set(32);");
    a("    ecl_time_sub(1);");
    a("    chapter_set(0);");
    a("    ecl_time_sub(1);");
    b(1, "__stone_value_set(1);");
    b(1, "GF0 = 0.0f;");
    b(1, "GF1 = 0.0f;");
    b(1, "GF2 = 0.2f;");
    b(1, "GF3 = 0.2f;");
    b(1, "GF4 = 0.2f;");
    b(1, "GF5 = 0.2f;");
    //b(1, "enemy_create_abs(\"RankSimulator\", 0.0f, 0.0f, 999999, 0, 0);");

    //Converts EoSD's timeline into a "main" enemy for FW
    for (int i = timelinePos; i < eosdSTG.Length; i++)
    {
        //Time label
        if (eosdSTG[i].Contains("+") && eosdSTG[i].Contains(": //"))
        {
            a(eosdSTG[i]);
        }
        else if (c(i, "ins_0(") && c(i, ");")) //Spawn enemy at absolute position
        {
            b(1, $"if (GI3 != 123) enemy_create_abs({ex(i, 2) /*Function*/}, {ex(i, 3) /*X*/} + SCREEN_FIX, {ex(i, 4) /*Y*/}, _S(_f({ex(i, 6) /*Life*/}) * 1.75f) + 62, {ex(i, 8) /*Score*/}, {GetItem(ex(i, 7)) /*Item*/} + 1);");

        }
        else if (c(i, "ins_2(") && c(i, ");")) //Spawn Mirrored enemy at absolute position
        {
            b(1, $"if (GI3 != 123) enemy_create_abs_mirror({ex(i, 2) /*Function*/}, {ex(i, 3) /*X*/} + SCREEN_FIX, {ex(i, 4) /*Y*/}, _S(_f({ex(i, 6) /*Life*/}) * 1.75f) + 62, {ex(i, 8) /*Score*/}, {GetItem(ex(i, 7)) /*Item*/} + 1);");

        }
        else if (c(i, "ins_4(") && c(i, ");")) //Spawn enemy at random position
        {
            b(1, $"if (GI3 != 123) enemy_create_abs({ex(i, 2) /*Function*/}, RAND_FLOAT_SIGNED * 192.0f, {ex(i, 4) /*Y*/}, _S(_f({ex(i, 6) /*Life*/}) * 1.75f) + 62, {ex(i, 8) /*Score*/}, {GetItem(ex(i, 7)) /*Item*/} + 1);");

        }
        else if (c(i, "ins_6(") && c(i, ");")) //Spawn Mirrored enemy at random position
        {
            b(1, $"if (GI3 != 123) enemy_create_abs_mirror({ex(i, 2) /*Function*/}, RAND_FLOAT_SIGNED * 192.0f, {ex(i, 4) /*Y*/}, _S(_f({ex(i, 6) /*Life*/}) * 1.75f) + 62, {ex(i, 8) /*Score*/}, {GetItem(ex(i, 7)) /*Item*/} + 1);");

        }
        else if (c(i, "ins_8(") && c(i, ");")) //Shows dialog
        {
            b(1, $"msg_read({ex(i, 2)});");
        }
        else if (c(i, "ins_9(") && c(i, ");")) //Waits for a dialog pause
        {
            b(1, $"msg_wait();");
        }
        else if (c(i, "ins_12(") && c(i, ");")) //Waits until there are no bosses
        {
            b(1, $"boss_wait();");
        }
        else if (s(i, "}")) //End of timeline
        {
            a("}");
            break;
        }
    }


    outsideTimeline = true;

    ScanEnemyFuncs();

    ScanBossEnd();

    ScanLasers();

    //Converts EoSD's subs into "voids" for FW (stops once it reaches the timeline)
    Console.WriteLine($"Timeline position after scans: {timelinePos}");
    for (int i = 0; i < timelinePos; i++)
    {
        Console.WriteLine($"Reading line: {i}");
        D();
        if (s(i, "!") && ex(i, 1).Length <= 6 && i < timelinePos) //Difficulty parameters
        {
            D();
            a(ex(i, 1));
            a("");
            eosdSTG[i] = eosdSTG[i].Replace(ex(i, 1), "");
        }
        D();
        if (c(i, "sub Sub") && c(i, "()")) //Functions
        {
            D();
            autoShoot = true;
            alreadySetSound = false;
            alreadySetEX = false;
            alreadySetComp = false;
            alreadySetBackup = false;
            alreadySetLaser = false;
            tempLaserMoves = false;
            alreadySetFuncs = false;
            dialogInterrupt = "";
            currentFunction = ex(i, 2).Replace("()", "");
            Console.WriteLine($"Processing Sub/Function: {currentFunction}");
            a("");
            if (enemyFunctions.Contains(ex(i, 2).Replace("()", ""))) //requires variables workaround
            {
                a(eosdSTG[i].Replace("sub ", "void "));
                a("{");
                VarsRetrieveWorkaround();
            }
            else
            {
                a(eosdSTG[i].Replace("sub ", "void ").Replace("()", "(int I0, int I1, int I2, int I3, int IC0, int IC1, int IC2, int IC3, float F0, float F1, float F2, float F3)"));
                a("{");
            }

            if (f(currentFunction, "effect_particle(3, 2, #80ff80ff);", ref dummyInt) && f(currentFunction, "enemy_life_set(0);", ref dummyInt) && f(currentFunction, "life_callback_threshold(-1);", ref dummyInt))
            {
                b(1, "@BossDead();");
            }

            //AddComparison();
            //SetFuncs();
            b(1, $"{((!alreadySetEX) ? "int" : "")} EXa = 0;");
            b(1, $"{((!alreadySetEX) ? "int" : "")} EXb = 0;");
            b(1, $"{((!alreadySetEX) ? "float" : "")} EXr = 0;");
            b(1, $"{((!alreadySetEX) ? "float" : "")} EXs = 0;");

            b(1, "EI0 = 0;");
            b(1, "EI1 = 0;");
            b(1, "EF0 = 0.0f;");
            b(1, "EF1 = 0.0f;");
            b(1, "EF2 = 0.0f;");
            b(1, "EF3 = 0.0f;");
            b(1, "int temp = 0;");

            lifeFunc = "";
            SetFuncs(0);

            if (laserFunctions.Contains(currentFunction))
            {
                for (int j = 0; j < 32; j++)
                {
                    b(1, $"float LASER{j}ANGLE = 0.0f;");
                }
            }


            alreadySetEX = true;

            if (bossEndFunctions.Contains(currentFunction)) //detects if the function is used to end the boss
            {
                b(1, "enemy_flags_set(1024);");
                b(1, "enemy_life_set(99999);");
                b(1, "enemy_kill_all();");
                b(1, "enemy_flags_clear(1024);");
                b(1, "GI3 = 0;");
                b(1, "if (!SPELL_TIMEOUT) effect_sound(5);");
                if (f(currentFunction, "enemy_delete(", ref dummyInt) && !f(currentFunction, "death_callback_sub", ref dummyInt))
                {
                    b(1, "effect_screen_shake(30, 12, 0);"); //screenshake if the enemy gets deleted afterwards
                }
            }

            BulletRankReset();
            a("");
            i++;
        }
        else if (s(i, "}")) //End of a function
        {
            D();
            a("}");
            a("");
        }
        else if ((c(i, "+") && c(i, ": //")) || s(i, "S") && e(i, ":")) //Time and regular labels
        {
            D();
            if (dialogInterrupt != "" && (c(i, "1000") || c(i, "10000")))
            {
                b(1, "msg_wait();");
                b(1, "chapter_set(43);");
                AddBackups(i);

                if (enemyFunctions.Contains(dialogInterrupt))
                {
                    b(1, $"@{dialogInterrupt}();");
                }
                else
                {
                    b(1, $"@{dialogInterrupt}(I0, I1, I2, I3, IC0, IC1, IC2, IC3, F0, F1, F2, F3);");
                }
            }
            else
            {
                a(eosdSTG[i]);
            }
        }
        else if (c(i, "enemy_delete(") && c(i, ");")) //deletes enemy if '1' and otherwise stops executing functions
        {
            D();
            b(1, $"if ({ex(i, 2)} == 1)");
            b(1, "{");
            b(2, "enemy_delete();");
            b(1, "}");
            b(1, "else");
            b(1, "{");
            b(2, "ecl_time_sub(999999);");
            b(1, "}");
        }
        else if ((c(i, "ret();") && !c(i + 1, "}")) || c(i, "nop();")) //return and nop
        {
            D();
            a(eosdSTG[i]);
        }
        else if (c(i, "jump(") && c(i, ");")) //jumps
        {
            D();
            b(1, $"goto {ex(i, 3)} @ {ex(i, 2)};");
        }
        else if (c(i, "loop(") && c(i, ");")) //loops
        {
            D();
            b(1, $"if ({ex(i, 4)} != 0)");
            b(1, "{");
            b(2, $"{ex(i, 4)} -= 1;");
            b(2, $"goto {ex(i, 3)} @ {ex(i, 2)};");
            b(1, "}");
        }
        else if ((c(i, "set_int(") || c(i, "set_float(")) && c(i, ");")) //set a variable
        {
            D();
            b(1, $"{ex(i, 2)} = {ex(i, 3)};");
        }
        else if (c(i, "set_int_rand_bound(") && c(i, ");")) //set a random value between 0 and smth
        {
            D();
            b(1, $"{ex(i, 2)} = RAND_INT % {ex(i, 3)};");
        }
        else if (c(i, "set_int_rand_bound_add(") && c(i, ");")) //set a random value between 0 and smth, then add another number (aka the minimum)
        {
            D();
            b(1, $"{ex(i, 2)} = (RAND_INT % {ex(i, 4)}) + {ex(i, 3)};");
        }
        else if (c(i, "set_float_rand_bound(") && c(i, ");")) //set a random FLOAT value between 0 and smth
        {
            D();
            b(1, $"{ex(i, 2)} = RAND_FLOAT * {ex(i, 3)};");
        }
        else if (c(i, "set_float_rand_bound_add(") && c(i, ");")) //set a random FLOAT value between 0 and smth, then add another number (aka the minimum)
        {
            D();
            b(1, $"{ex(i, 2)} = (RAND_FLOAT * {ex(i, 4)}) + {ex(i, 3)};");
        }
        else if (c(i, "set_var_self_x(") & c(i, ");")) //get self x
        {
            D();
            b(1, $"{ex(i, 2)} = _S(SELF_X);");
        }
        else if (c(i, "set_var_self_y(") & c(i, ");")) //get self y
        {
            D();
            b(1, $"{ex(i, 2)} = _S(SELF_Y);");
        }
        else if (c(i, "set_var_self_z(") & c(i, ");")) //get self z (0 since i can't find it lol)
        {
            D();
            b(1, $"{ex(i, 2)} = 0;");
        }
        else if (c(i, "math_") && c(i, "_add(") & c(i, ");")) //adds 2 values and stores the result in a variable
        {
            D();
            b(1, $"{ex(i, 2)} = {ex(i, 3)} + {ex(i, 4)};");
        }
        else if (c(i, "math_") && c(i, "_sub(") & c(i, ");")) //subtracts 2 values and stores the result in a variable
        {
            D();
            b(1, $"{ex(i, 2)} = {ex(i, 3)} - {ex(i, 4)};");
        }
        else if (c(i, "math_") && c(i, "_mul(") & c(i, ");")) //multiplies 2 values and stores the result in a variable
        {
            D();
            b(1, $"{ex(i, 2)} = {ex(i, 3)} * {ex(i, 4)};");
        }
        else if (c(i, "math_") && c(i, "_div(") & c(i, ");")) //divides 2 values and stores the result in a variable
        {
            D();
            b(1, $"{ex(i, 2)} = {ex(i, 3)} / {ex(i, 4)};");
        }
        else if (c(i, "math_") && c(i, "_mod(") & c(i, ");")) //gets the mod of 2 values and stores the result in a variable
        {
            D();
            b(1, $"{ex(i, 2)} = {ex(i, 3)} % {ex(i, 4)};");
        }
        else if (c(i, "math_inc(") && c(i, ");")) //increments a variable by 1
        {
            D();
            b(1, $"{ex(i, 2)} += {(ex(i, 2).Contains("F") || ex(i, 2).Contains("%") ? "1.0f" : "1")};");
        }
        else if (c(i, "math_dec(") && c(i, ");")) //decrements a variable by 1
        {
            D();
            b(1, $"{ex(i, 2)} -= {(ex(i, 2).Contains("F") || ex(i, 2).Contains("%") ? "1.0f" : "1")};");
        }
        else if (c(i, "math_line_angle(") && c(i, ");")) //gets the angle between 2 points
        {
            D();
            a(eosdSTG[i]);
        }
        else if (c(i, "math_reduce_angle(") && c(i, ");")) //reduces an angle until its between -pi and pi
        {
            D();
            a(eosdSTG[i]);
        }
        else if (c(i, "ex_ins_call")) //hardcoded functions
        {
            D();
            Hardcoded(i);
        }
        else if ((c(i, "cmp_int(") || c(i, "cmp_float(")) && c(i, ");")) //tests 2 values
        {
            D();
            b(1, $"if ({ex(i, 2)} < {ex(i, 3)}) EI2 = -1;");
            b(1, $"if ({ex(i, 2)} == {ex(i, 3)}) EI2 = 0;");
            b(1, $"if ({ex(i, 2)} > {ex(i, 3)}) EI2 = 1;");
        }
        else if (c(i, "jump_") && c(i, ");")) //conditional jumps
        {
            D();
            b(1, $"if (EI2 {insNumbers[ex(i, 1)]}");
            b(1, "{");
            b(2, $"goto {ex(i, 3)} @ {ex(i, 2)};");
            b(1, "}");
        }
        else if (c(i, "call(") && c(i, ");")) //calls
        {
            D();
            if (AddBackups(i))
            {
                b(1, $"EF0 = _f({ex(i, 3)}) + 0.2f;");
                b(1, $"EI0 = _S({ex(i, 4)} * 1000000.0f);");
            }

            if (enemyFunctions.Contains(ex(i, 2).Replace("\"", "")))
            {
                b(1, $"@{ex(i, 2).Replace("\"", "")}();");
            }
            else
            {
                b(1, $"@{ex(i, 2).Replace("\"", "")}({ex(i, 3)}, I1, I2, I3, IC0, IC1, IC2, IC3, {ex(i, 4)}, F1, F2, F3);");
            }
        }
        else if (c(i, "call_") && c(i, ");")) //conditional calls
        {
            D();
            b(1, $"if ({ex(i, 5)} < {ex(i, 6)}) EI2 = -1;");
            b(1, $"if ({ex(i, 5)} == {ex(i, 6)}) EI2 = 0;");
            b(1, $"if ({ex(i, 5)} > {ex(i, 6)}) EI2 = 1;");
            b(1, $"if (EI2 {insNumbers[ex(i, 1)]}");
            b(1, "{");
            if (AddBackups(i))
            {
                b(1, $"EF0 = _f({ex(i, 3)}) + 0.2f;");
                b(1, $"EI0 = _S({ex(i, 4)} * 1000000.0f);");
            }

            if (enemyFunctions.Contains(ex(i, 2).Replace("\"", "")))
            {
                b(1, $"@{ex(i, 2).Replace("\"", "")}();");
            }
            else
            {
                b(1, $"@{ex(i, 2).Replace("\"", "")}({ex(i, 3)}, I1, I2, I3, IC0, IC1, IC2, IC3, {ex(i, 4)}, F1, F2, F3);");
            }
            b(1, "}");
        }
        else if (c(i, "move_position(") && c(i, ");")) //sets the enemy's position
        {
            D();
            b(1, $"move_position_abs({ex(i, 2)} + SCREEN_FIX, {ex(i, 3)});");
        }
        else if (c(i, "anm_set_main(") && c(i, ");")) //anm scripts
        {
            D();
            switch (ex(i, 2))
            {
                case "0": //blue fairies
                    b(1, "anm_source(2);");
                    b(1, "anm_set_slot_main(0, 0);");
                    b(1, "anm_set_slot(1, 323);");
                    break;

                case "3": //pink/red fairies
                    b(1, "anm_source(2);");
                    b(1, "anm_set_slot_main(0, 5);");
                    b(1, "anm_set_slot(1, 323);");
                    break;

                //feathers? (spirits/phantoms in FW)
                case "12":
                    b(1, "anm_source(2);");
                    b(1, "anm_set_slot(0, 87);");
                    break;

                //yin-yangs (or smth like that?) in FW
                case "13":
                    b(1, "anm_source(2);");
                    //chooses color based on stage:
                    switch (stage)
                    {
                        case 2:
                            b(1, "anm_set_slot(0, 59);");
                            break;

                        case 3:
                            b(1, "anm_set_slot(0, 56);");
                            break;

                        case 4:
                            b(1, "anm_set_slot(0, 62);");
                            break;

                        case 5:
                            b(1, "anm_set_slot(0, 53);");
                            break;

                        case 6:
                            b(1, "anm_set_slot(0, 53);");
                            break;

                        case 7:
                            b(1, "anm_set_slot(0, 53);");
                            break;
                    }
                    //b(1, "anm_set_slot(1, 323);");
                    break;


                case "64": //daiyousei
                    BossANM(i, stage);
                    break;
            }
        }
        else if (c(i, "drop_items("))
        {
            D();
            if (bossEndFunctions.Contains(currentFunction)) //used when an enemy explodes
            {
                b(1, $"item_bonus_count_reset();");
                b(1, $"item_drop_area(48.0f, 48.0f);");
                b(1, $"item_bonus_count_set({BossItemDrops()});");
                b(1, $"drop_item_rewards();");
                b(1, "effect_sound(5);");
                if (f(currentFunction, "enemy_delete(", ref dummyInt) && !f(currentFunction, "death_callback_sub", ref dummyInt))
                {
                    b(1, "effect_screen_shake(30, 12, 0);"); //screenshake if the enemy gets deleted afterwards
                }
            }
            else
            {

            }
        }
        else if (c(i, "enemy_life_set(")) //sets enemy's life
        {
            D();
            b(1, $"if ({ex(i, 2)} == 0)");
            b(1, "{");
            b(2, "enemy_delete();");
            b(1, "}");
            b(1, "else");
            b(1, "{");
            b(2, $"enemy_life_set(_S(_f({ex(i, 2) /*Life*/}) * 2.25f) + 62);");
            b(1, "}");
        }
        else if (c(i, "enemy_create(") && c(i, ");")) //creates a new enemy
        {
            D();
            VarsStoreWorkaround();
            b(1, $"enemy_create_abs({ex(i, 2) /*Function*/}, {ex(i, 3) /*X*/} + SCREEN_FIX, {ex(i, 4) /*Y*/}, {ex(i, 6) /*Life*/}, {ex(i, 8) /*Score*/}, {GetItem(ex(i, 7)) /*Item*/} + 1);");
        }
        else if (c(i, "enemy_set_hitbox(") && c(i, ");")) //hitbox and hurtbox set
        {
            D();
            b(1, $"enemy_set_hitbox({ex(i, 2)}, {ex(i, 3)});");
            b(1, $"enemy_set_collision({ex(i, 2)}, {ex(i, 3)});");
        }
        else if (c(i, "move_velocity(") && c(i, ");")) //set direction and speed
        {
            D();
            a(eosdSTG[i].Replace("move_velocity", "move_velocity_abs"));
        }
        else if (c(i, "move_angular_velocity(") && c(i, ");")) //set direction and speed
        {
            D();
            b(1, $"move_angle_abs_interp(99999, 7, {ex(i, 2)});");
        }
        else if (c(i, "move_acceleration(") && c(i, ");")) //set direction and speed
        {
            D();
            b(1, $"move_speed_abs_interp(99999, 7, {ex(i, 2)});");
        }
        else if (c(i, "move_rand_in_bounds(-3.1415927f, 3.1415927f);") && c(i + 1, "move_speed(") && c(i + 2, "move_as_interp_decelerate")) //simplifying stuff basically
        {
            D();
            b(1, $"move_rand_interp_abs({ex(i + 2, 2)}, 4, {ex(i + 1, 2)});");
            i += 2;
        }
        else if (c(i, "move_speed") && c(i, ");")) //set speed
        {
            D();
            a(eosdSTG[i].Replace("move_speed", "move_speed_abs"));
        }
        else if (c(i, "move_rand")) //sets a random rotation
        {
            D();
            b(1, $"move_angle_abs(RAND_FLOAT * ({ex(i, 3)} - {ex(i, 2)}) + {ex(i, 2)});");
        }
        else if (c(i, "move_towards_player")) //point to the player + angle with certain speed
        {
            D();
            b(1, $"move_velocity_abs(PLAYER_ANGLE + {ex(i, 2)}, {ex(i, 3)});");
        }
        else if (c(i, "move_as_interp_decelerate(") && c(i, ");")) //decelerates with current speed and angle for x amount of frames
        {
            D();
            b(1, $"move_speed_abs_interp({ex(i, 2)}, 4, 0.0f);");
        }
        else if (c(i, "move_position_interp_decelerate(") && c(i, ");")) //decelerates towards some coordinates
        {
            D();
            b(1, $"move_position_abs_interp({ex(i, 2)}, 4, {ex(i, 3)} + SCREEN_FIX, {ex(i, 4)});");
        }
        else if (c(i, "move_position_interp_accelerate(") && c(i, ");")) //decelerates towards some coordinates
        {
            D();
            b(1, $"move_position_abs_interp({ex(i, 2)}, 2, {ex(i, 3)} + SCREEN_FIX, {ex(i, 4)});");
        }
        else if (c(i, "move_rand_in_bounds(") && c(i, ");")) //random angle with range
        {
            D();
            b(1, $"move_angle_abs((RAND_FLOAT * ({ex(i, 2)} + {ex(i, 3)})) + {ex(i, 2)});");
        }
        else if (c(i, "move_bounds_set(") && c(i, ");")) //enables move limit
        {
            D();
            b(1, $"move_bounds_set((({ex(i, 4)} - {ex(i, 2)}) / 2.0f) + SCREEN_FIX, (({ex(i, 5)} - {ex(i, 3)}) / 2.0f) + 64.0f, ({ex(i, 4)} - {ex(i, 2)}) - 32.0f, ({ex(i, 5)} - {ex(i, 3)}) - 48.0f);");
        }
        else if (c(i, "move_bounds_disable();")) //disables move limit
        {
            D();
            a(eosdSTG[i]);
        }
        else if (c(i, "enemy_flag_collision(") && c(i, ");")) //toggles hitbox
        {
            D();
            b(1, $"if ({ex(i, 2)} % 2 == 1)");
            b(1, "{");
            b(2, "enemy_flags_clear(1);");
            b(1, "}");
            b(1, "else");
            b(1, "{");
            b(2, "enemy_flags_set(1);");
            b(1, "}");
        }
        else if (c(i, "enemy_flag_collision(") && c(i, ");")) //toggles hurtbox
        {
            D();
            b(1, $"if ({ex(i, 2)} % 2 == 1)");
            b(1, "{");
            b(2, "enemy_flags_clear(2);");
            b(1, "}");
            b(1, "else");
            b(1, "{");
            b(2, "enemy_flags_set(2);");
            b(1, "}");
        }
        else if (c(i, "death_callback_sub(") && c(i, ");")) //function to go to after hp is 0
        {
            D();
            SetFuncs(3);
            //b(1, $"death_callback_sub({ex(i, 2)});");
            b(1, $"callback_ex(1, 0, 0, {ex(i, 2)});");
        }
        else if (c(i, "life_callback_threshold(") && c(i, ");"))
        {
            D();
            SetFuncs(2);
            lifeInt = ex(i, 2);
            b(1, $"temp = {ex(i, 2)};");
        }
        else if (c(i, "life_callback_sub(") && c(i, ");"))
        {
            D();
            SetFuncs(2);
            //lifeInt = ex(i, 2).Replace("\"", "");
            //b(1, $"callback_ex(0, LIFEINT, TIMEINT, \"L{lifeInt.Replace("Sub", "")}T{timeInt.Replace("Sub", "")}\");");
            lifeFunc = ex(i, 2);
            b(1, $"callback_ex(0, temp, 0, {ex(i, 2)});");
            b(1, $"enemy_lifebar_color(0, _f(temp), 0);");
            //CheckCallbacks();
        }
        else if (c(i, "timer_callback_threshold(") && c(i, ");"))
        {
            D();
            SetFuncs(1);
            b(1, $"temp = {ex(i, 2)};");
            if (!f(currentFunction, "timer_callback_sub", ref dummyInt))
            {
                exitInsideLoop = false;
                for (int j = 0; j < FuncsFound.Count && !exitInsideLoop; j++)
                {
                    if (f(FuncsFound[j], $"timer_callback_sub(\"{currentFunction}\")", ref tmpInt) && f(FuncsFound[j], "death_callback_sub(", ref dummyInt))
                    {
                        temp = ex(dummyInt, 2);

                        b(2, $"callback_ex(1, 0, temp, {ex(dummyInt, 2)});");

                        //b(2, $"if ((EI3 & 2) == 2) callback_ex(0, {lifeInt}, temp, {(lifeFunc == "" ? "\"\"" : lifeFunc)});");
                        b(2, $"timer_callback_sub(1, {temp});");

                        exitInsideLoop = true;
                    }
                }
            }
        }
        else if (c(i, "timer_callback_sub(") && c(i, ");"))
        {
            D();
            SetFuncs(1);
            if (f(currentFunction, "death_callback_sub(", ref dummyInt))
            {
                b(2, $"if ((EI3 & 4) == 4) callback_ex(1, 0, temp, {ex(dummyInt, 2)});");
            }
            b(2, $"if ((EI3 & 2) == 2) callback_ex(0, {(lifeInt == "" ? "0" : lifeInt)}, temp, {(lifeFunc == "" ? "\"\"" : lifeFunc)});");
            b(2, $"timer_callback_sub(1, {ex(i, 2)});");
            //b(2, $"callback_ex(1, LIFEINT, TIMEINT, {ex(i, 2)});");


            /*b(1, $"if (LIFEINT <= 0)");
            b(1, "{");

            b(1, "}");
            b(1, "else");
            b(1, "{");
            b(2, $"timer_callback_sub(0, {ex(i, 2)});");
            b(1, "}");*/
            //timeInt = ex(i, 2).Replace("\"", "");
            //b(1, $"callback_ex(0, LIFEINT, TIMEINT, \"L{lifeInt.Replace("Sub", "")}T{timeInt.Replace("Sub", "")}\");");
            //CheckCallbacks();
        }
        else if (c(i, "boss_set(") && c(i, ");")) //sets the boss
        {
            D();
            a(eosdSTG[i]);
            b(1, "enemy_flags_set(1024);");
            b(1, "enemy_kill_all_stones();");
            b(1, "enemy_kill_all();");
            b(1, "GI3 = 123;");
        }
        else if (c(i, "enemy_interrupt_set(")) //boss interrupts
        {
            D();
            if (ex(i, 3) == "0")
            {
                dialogInterrupt = ex(i, 2).Replace("\"", "");
            }
            else //idk yet, i'll have to research
            {

            }
            //b(1, $"callback_ex({ex(i, 3)} + 3, -999999, -999999, {ex(i, 2)});");
        }
        else if ((c(i, "anm_set_poses(") && c(i, ");")) || (c(i, "anm_set_main") && f(currentFunction, "boss_set(", ref dummyInt))) //boss sprites
        {
            BossANM(i, stage);
        }
        else if (c(i, "anm_set_main(") && c(i, ");")) //attack animation
        {
            b(1, "anm_play_attack(0);");
        }
        else if (c(i, "boss_set_life_count(") && c(i, ");")) //boss' stars
        {
            a(eosdSTG[i]);
        }
        else if (c(i, "bullet_rank_influence(")) //rank
        {
            b(1, $"GF0 = {ex(i, 2)};");
            b(1, $"GF1 = {ex(i, 3)};");
            b(1, $"GF2 = _f({ex(i, 4)}) + 0.2f;");
            b(1, $"GF3 = _f({ex(i, 5)}) + 0.2f;");
            b(1, $"GF4 = _f({ex(i, 6)}) + 0.2f;");
            b(1, $"GF5 = _f({ex(i, 7)}) + 0.2f;");
        }
        else if (c(i, "shoot_disable();")) //disables automatic shooting after setting attributes
        {
            autoShoot = false;
        }
        else if (c(i, "shoot_enable();")) //enables automatic shooting after setting attributes or immediately if it was turned off before
        {
            if (!autoShoot && !c(i + 1, "shoot_interval_delayed"))
            {
                b(1, "shoot_now(0);");
            }
            autoShoot = true;

        }
        else if (c(i, "shoot_interval(") && c(i, ");")) //shoot bullets automatically
        {
            b(1, "async_stop_id(123);");
            if (ex(i, 2) != "0")
            {
                b(1, $"@Auto({ex(i, 2)}) async 123;");
            }
        }
        else if (c(i, "shoot_interval_delayed(") && c(i, ");")) //shoot bullets automatically in random intervals
        {
            b(1, "async_stop_id(124);");
            if (ex(i, 2) != "0")
            {
                b(1, "async_stop_id(124);");
                b(1, $"@AutoDelay({ex(i, 2)}) async 124;");
            }
        }
        else if (c(i, "shoot_offset(") && c(i, ");")) //shooting offset
        {
            b(1, $"EF6 = {ex(i, 2)};");
            b(1, $"EF7 = {ex(i, 2)};");
            b(1, $"shoot_offset(0, {ex(i, 2)}, {ex(i, 3)});");
        }
        else if (c(i, "bullet_effects(") && c(i, ");")) //bullet effects attributes
        {
            b(1, $"{((!alreadySetEX) ? "int" : "")} EXa = {ex(i, 2)};");
            b(1, $"{((!alreadySetEX) ? "int" : "")} EXb = {ex(i, 3)};");
            b(1, $"{((!alreadySetEX) ? "float" : "")} EXr = {ex(i, 6)};");
            b(1, $"{((!alreadySetEX) ? "float" : "")} EXs = {ex(i, 7)};");
            alreadySetEX = true;
        }
        else if (c(i, "bullet_") && c(i, ");")) //bullet aim modes
        {
            if (argsFind(i) == 9) //used to discard other functions like bullet_cancel, bullet_sounds, etc
            {
                b(1, $"EI0 = {ex(i, 4)} + _S(GF3);"); //(16 * (_S(GF3) - _S(GF2) ) / 32 + _S(GF2));");

                b(1, $"EI1 = {ex(i, 5)} + _S(GF5);"); //(16 * (_S(GF5) - _S(GF4) ) / 32 + _S(GF4));");

                b(1, $"EF0 = {ex(i, 6)} + GF1;"); //(_f(16) * (GF1 - GF0 ) / 32.0f + GF0);");


                b(1, $"EF1 = {ex(i, 7)} + GF1;");//((_f(16) * (GF1 - GF0 ) / 32.0f + GF0));");

                b(1, $"EF2 = {ex(i, 8)};");
                b(1, $"EF3 = {ex(i, 9)};");

                b(1, "math_reduce_angle(EF2);");
                b(1, "math_reduce_angle(EF3);");

                b(1, "shooter_reset(0);");
                b(1, "shoot_offset(0, EF6, EF7);");
                b(1, $"shoot_aim_mode(0, {insNumbers[ex(i, 1)]});");
                b(1, $"bullet_sprite(0, {convertBullet(ex(i, 2))}, {ex(i, 3)});");
                b(1, $"bullet_count(0, (EI0 < 1) ? 1 : EI0, (EI1 < 1) ? 1 : EI1);");//_S(sqrt({RankFormula(1)} * {RankFormula(1)})) + {ex(i, 4)}, _S(sqrt({RankFormula(2)} * {RankFormula(2)})) + {ex(i, 5)});");
                b(1, $"bullet_speed(0, (EF0 < 0.3f) ? 0.3f : EF0, (EF1 < 0.3f) ? 0.3f : EF1);");//({RankFormula(0)} * {ex(i, 6)}) + {ex(i, 6)}, ({RankFormula(0)} * {ex(i, 7)}) + {ex(i, 7)});");
                b(1, $"shoot_angle(0, EF2, EF3);");
                tmpInt = int.Parse(ex(i, 10)); //flags

                if (isFlagPresent(tmpInt, 2)) //bullet cloud (small)
                {
                    b(1, $"bullet_effects_add(0, 0, 1, 0, -1, -1.0f, -1.0f);");
                }
                else if (isFlagPresent(tmpInt, 4)) //(medium)
                {
                    b(1, $"bullet_effects_add(0, 0, 1, 1, -1, -1.0f, -1.0f);");
                }
                else if (isFlagPresent(tmpInt, 8)) //(large)
                {
                    b(1, $"bullet_effects_add(0, 0, 1, 2, -1, -1.0f, -1.0f);");
                }

                if (isFlagPresent(tmpInt, 1)) //quick speed once launched
                {
                    b(1, $"bullet_effects_add(0, 0, 0, -1, -1, -1.0f, -1.0f);");
                }

                if (!isFlagPresent(tmpInt, 512)) //disable sound if not present
                {
                    b(1, $"bullet_sound(0, -1, -1);");
                }
                else if (!alreadySetSound)
                {
                    b(1, $"bullet_sound(0, 21, 38);");
                }

                if (!alreadySetEX) //checks if the ex flags attributes variables were initialized
                {
                    b(1, "int EXa = 0;");
                    b(1, "int EXb = 0;");
                    b(1, "float EXr = 0.0f;");
                    b(1, "float EXs = 0.0f;");
                    alreadySetEX = true;
                }

                //flags that require attributes:
                if (isFlagPresent(tmpInt, 16)) //rotate to point to a direction
                {
                    b(1, $"if (EXs < -998.0f)");
                    b(1, "{");
                    b(2, "bullet_effects_add(0, 0, 3, (EXa < 0) ? 99999 : EXa, -1, EXr, 0.0f);");
                    b(1, "}");
                    b(1, "else");
                    b(1, "{");
                    b(2, $"bullet_effects_add(0, 0, 2, (EXa < 0) ? 99999 : EXa, -1, EXr, EXs);");
                    b(1, "}");
                }
                else if (isFlagPresent(tmpInt, 32)) //accelerate with certain speed and angle
                {
                    b(1, $"bullet_effects_add(0, 0, 3, (EXa < 0) ? 99999 : EXa, -1, EXs, EXr);");
                }
                else if (isFlagPresent(tmpInt, 64)) //stop, change angle and set speed
                {
                    b(1, $"bullet_effects_add(0, 0, 4, EXa, EXb, EXr, EXs + GF1);");
                }
                else if (isFlagPresent(tmpInt, 128)) //stop, aim to the player, change angle and set speed
                {
                    b(1, $"bullet_effects_add_ex(0, 0, 4, EXa, EXb, 1, 0, EXr, EXs + GF1, -1.0f, -1.0f);");
                }
                else if (isFlagPresent(tmpInt, 256)) //stop, set angle and speed
                {
                    b(1, $"bullet_effects_add_ex(0, 0, 4, EXa, EXb, 4, 0, EXr, EXs + GF1, -1.0f, -1.0f);");
                }
                else if (isFlagPresent(tmpInt, 1024)) //bounce on all walls
                {
                    b(1, $"bullet_effects_add(0, 0, 6, EXa, 15, EXr, -1.0f);");
                }
                else if (isFlagPresent(tmpInt, 1024)) //bounce on non-bottom walls
                {
                    b(1, $"bullet_effects_add(0, 0, 6, EXa, 13, EXr, -1.0f);");
                }

                if (autoShoot)
                {
                    b(1, "shoot_now(0);");
                }
            }
        }
        else if (c(i, "laser_index(") && c(i, ");")) //sets the laser index to use
        {
            D();
            b(1, $"{((!alreadySetLaser) ? "int" : "")} laserID = {ex(i, 2)};");

            alreadySetLaser = true;
        }
        else if (c(i, "laser_rotate(") && c(i, ");")) //rotates a laser
        {
            D();
            LaserRotateFunc(ex(i, 2), ex(i, 3), true);
            //a(eosdSTG[i].Replace("laser_rotate", "@LaserRotate").Replace(";", " async;"));
        }
        else if (c(i, "laser_offset(") && c(i, ");")) //sets the position of a laser
        {
            D();
            b(1, $"laser_offset({ex(i, 2)}, {ex(i, 3)} + SCREEN_FIX, {ex(i, 4)});");
        }
        else if (c(i, "laser_cancel(") && c(i, ");")) //cancels a laser
        {
            D();
            a(eosdSTG[i].Replace("laser_cancel", "laser_clear"));
        }
        else if (c(i, "laser_create(") && c(i, ");")) //creates a laser
        {
            D();
            CreateLaser(i, ex(i, 4));
        }
        else if (c(i, "laser_create_aimed(") && c(i, ");")) //creates a laser aimed to the player
        {
            D();
            CreateLaser(i, "PLAYER_ANGLE");
        }
        else if (c(i, "spellcard_end();"))//ends a spellcard
        {
            D();
            b(1, "@SpellEnd();");
        }
        else if (c(i, "spellcard_start(") && c(i, ");")) //starts a spellcard
        {
            D();
            b(1, $"spellcard_start({ex(i, 3)}, 6000, 0, {ex(i, 4)});");
            //b(1, "callback_ex(0, -1, 0, \"\");");
            b(1, "enemy_invincible_timer(60);");
            b(1, "SPELL_CAPTURE = 1;");
            b(1, "PLAYER_DEATHS = 0;");
            b(1, "PLAYER_BOMBS_USED = 0;");
            b(1, $"enemy_kill_all_stones();");
            b(1, "bullet_cancel_radius(640.0f);");
            b(1, "laser_clear_all();");
            b(1, "if (SELF_LIFE <= 0) enemy_life_set(1750);");
            b(1, $"enemy_life_set(_S(_f((SELF_LIFE > 501) ? 500 : SELF_LIFE) * 3.5f));"); //damage reduction
            b(1, "if (SELF_LIFE <= 0) enemy_life_set(1750);");
            b(1, $"enemy_lifebar_color(0, 999999.0f, 0);");
            BulletRankReset();
        }
        else if (c(i, "effect_sound(") && c(i, ");")) //sound effects (SFX)
        {
            D();
            b(1, $"effect_sound({ConvertSFX(ex(i, 2))});");
        }

        D();
    }


    //AddCallbacks();

    //ChangeVarMaps(); //a few variable adjustments (NOT USED ANYMORE xd)
    File.WriteAllLines($"modded{Path.DirectorySeparatorChar}st0{stage}.txt", fwSTG.ToArray());
    Console.WriteLine("Done!");
    Environment.Exit(0);
}

void Hardcoded(int i) //TODO: Add cirno's "Perfect Freeze" function
{
    if (c(i, "ex_ins_call")) //with argument
    {
        switch (ex(i, 2))
        {
            case "1": //set a random offset for the bullet shooter
                b(1, $"EF6 = RAND_FLOAT_SIGNED * _f({ex(i, 3)});");
                b(1, $"EF7 = RAND_FLOAT_SIGNED * _f({ex(i, 3)});");
                b(1, $"shoot_offset(0, EF6, EF7);");
            break;
        }
    }
    else //without argument
    {
        switch (ex(i, 2))
        {

        }
    }
}


void BossANM(int i, int stage)
{
    D();
    b(i, "anm_source(1);"); //magic circle
    b(i, "anm_set_slot(1, 99);");

    b(i, "anm_source(3);"); //the actual sprite
    b(i, "anm_set_slot_main(0, 0);");

    //TODO: Add logic when midbosses aren't the final boss (afaik its daiyousei, koakuma, sakuya in stg6, and patchouli in extra)

    //fogs:
    if (c(i, "128, 131, 132, 129, 130")) //rumia
    {
        b(1, "enemy_fog_spawn(128.0f, 16711680);"); //red
    }
    else if (c(i, "128, 129, 130, 129, 130")) //cirno
    {
        b(1, "enemy_fog_spawn(128.0f, 7312127);"); //blue
    }
    //this was added here to reduce a bit of the work to the cpu (might not be much compared to other functions xdd, at least is smth :'v)
    else if (!f(currentFunction, "anm_set_poses", ref dummyInt) && stage == 2) //daiyousei
    {
        b(1, "enemy_fog_spawn(128.0f, 6934423);"); //green
    }
}

//unused:
/*
void AddCallbacks()
{
    for (int i = 0; i < callbacks.Count; i++)
    {
        a("");
        a($"void L{callbacks[i].lifeFunc.Replace("Sub", "")}T{callbacks[i].timeoutFunc.Replace("Sub", "")}()");
        a("{");
        b(1, "if (SPELL_TIMEOUT)");
        b(1, "{");
        if (AddBackupsAlt(callbacks[i].timeoutFunc))
        {
            b(2, $"@{callbacks[i].timeoutFunc}();");
        }
        else
        {
            b(2, $"@{callbacks[i].timeoutFunc}(0, 0, 0, 0, 0, 0, 0, 0, 0.0f, 0.0f, 0.0f, 0.0f);");
        }
        b(1, "}");
        b(1, "else");
        b(1, "{");
        if (AddBackupsAlt(callbacks[i].lifeFunc))
        {
            b(2, $"@{callbacks[i].lifeFunc}();");
        }
        else
        {
            b(2, $"@{callbacks[i].lifeFunc}(0, 0, 0, 0, 0, 0, 0, 0, 0.0f, 0.0f, 0.0f, 0.0f);");
        }
        //b(2, $"@{callbacks[i].lifeFunc}();");
        b(1, "}");
        a("}");
        a("");
    }
}
*/

/*void CheckCallbacks()
{
    if (lifeInt != "" && timeInt != "") //both life and timeout functions are set
    {
        callbacks.Add(new MultiCallback() { lifeFunc = lifeInt, timeoutFunc = timeInt });
    }
}*/

void ScanLasers() //looks for lasers' creation and rotation uses
{
    List<string> possibleFuncs = new List<string>();
    string currentReading = "";
    laserFunctions.Clear();
    for (int i = 0; i < timelinePos; i++) //looks for any "laser_create*" functions
    {
        if (c(i, "sub Sub")) //finds a function
        {
            currentReading = ex(i, 2).Replace("()", "").Replace(" ", "");
            Console.WriteLine($"Parsing function [{currentReading}] looking for lasers");
        }

        if (c(i, "laser_create") && !possibleFuncs.Contains(currentReading))
        {
            possibleFuncs.Add(currentReading);
        }
    }

    for (int i = 0; i < timelinePos; i++) //looks for any "laser_rotate()" functions
    {
        if (c(i, "sub Sub")) //finds a function
        {
            currentReading = ex(i, 2).Replace("()", "").Replace(" ", "");
        }

        if (c(i, "laser_rotate(") && possibleFuncs.Contains(currentReading) && !laserFunctions.Contains(currentReading))
        {
            Console.WriteLine($"Function [{currentReading}] uses Lasers and Rotation!");
            laserFunctions.Add(currentReading);
        }
    }
}

//arg:
//0 = Speed
//1 = Amount 1
//2 = Amount 2
string RankFormula(int arg) //old, unused, didn't work
{
    switch (arg)
    {
        case 0:
            //return $"(({Rank32} * (GF1 - GF0)) + GF0)"; //  + CURRENT_BULLET_VALUE
            break;

        case 1:
            //return $"(({Rank32} * (GF3 - GF2)) + GF0)"; //  + CURRENT_BULLET_VALUE
            break;

        case 2:
            //return $"(({Rank32} * (GF5 - GF4)) + GF0)"; //  + CURRENT_BULLET_VALUE
            break;
    }
    //return $"(({Rank32} * (HIGH - LOW)) + LOW)"; //  + CURRENT_BULLET_VALUE
    return ""; //just so it doesn't throw an error xd
}

void BulletRankReset()
{
    //rank influence reset:
    b(1, $"GF0 = -0.5f;");
    b(1, $"GF1 = 0.5f;");
    b(1, $"GF2 = 0.2f;");
    b(1, $"GF3 = 0.2f;");
    b(1, $"GF4 = 0.2f;");
    b(1, $"GF5 = 0.2f;");
}

string BossItemDrops()
{
    return "1, 10"; //10 power items
}

void LaserRotateFunc(string id, string ang, bool increase) //adds some weird logic workaround to change relative angles in lasers on line 'l'
{
    b(1, $"switch({id})");
    b(1, "{");
    for (int i = 0; i < 32; i++)
    {
        b(2, $"case {i}:");
        if (increase)
        {
            b(3, $"laser_angle({i}, LASER{i}ANGLE + {ang});");
            b(3, $"LASER{i}ANGLE += {ang};");
        }
        else
        {
            b(3, $"laser_angle({i}, {ang});");
            b(3, $"LASER{i}ANGLE = {ang};");
        }
        b(2, "break;");
        b(2, "");
    }
    b(1, "}");
}

void ScanBossEnd() //checks every function
{
    List<string> possibleFuncs = new List<string>();
    string currentReading = "";
    int dummy = 0;

    for (int i = 0; i < FuncsFound.Count; i++)
    {
        Console.WriteLine($"Scanning function [{FuncsFound[i]}] for Boss-End instructions");
        if (f(FuncsFound[i], "death_callback_sub", ref dummy))
        {
            currentReading = ex(dummy, 2).Replace("\"", "");
            Console.WriteLine($"Function [{currentReading}] may have Boss-End instructions");
            if (f(currentReading, "enemy_delete", ref dummy) && (f(currentReading, "effect_particle(3, 2, #ffffffff);", ref dummy) || f(currentReading, "spellcard_end();", ref dummy)))
            {
                Console.WriteLine($"Detected Boss-End Function: {currentReading}");
                if (!bossEndFunctions.Contains(currentReading)) bossEndFunctions.Add(currentReading);
            }
        }

        if (f(FuncsFound[i], "timer_callback_sub", ref dummy))
        {
            currentReading = ex(dummy, 2).Replace("\"", "");
            Console.WriteLine($"Function [{currentReading}] may have Boss-End instructions");
            if (f(currentReading, "enemy_delete", ref dummy) && (f(currentReading, "effect_particle(3, 2, #ffffffff);", ref dummy) || f(currentReading, "spellcard_end();", ref dummy)))
            {
                Console.WriteLine($"Detected Boss-End Function: {currentReading}");
                if (!bossEndFunctions.Contains(currentReading)) bossEndFunctions.Add(currentReading);
            }
        }



    }

    //old code before f();
    /*
    for (int i = 0; i < timelinePos; i++) //looks for death_callback_sub and extracts its function's name (or timer_callback_sub for timeouts "escapes")
    {
        if (c(i, "sub Sub")) //finds a function
        {
            currentReading = ex(i, 2).Replace("()", "").Replace(" ", "");
            Console.WriteLine($"Scanning {currentReading} for Boss-End function");
        }

        if ((c(i, "timer_callback_sub(")) || (c(i, "death_callback_sub(") && !possibleFuncs.Contains(ex(i, 2).Replace("\"", ""))))
        {
            Console.WriteLine($"Possible Boss-End function: {ex(i, 2).Replace("\"", "")}");
            possibleFuncs.Add(ex(i, 2).Replace("\"", "").Replace(" ", ""));
        }
    }

    for (int i = 0; i < timelinePos; i++) //confirms if the function has enemy_delete
    {
        if (c(i, "sub Sub")) //finds a function
        {
            currentReading = ex(i, 2).Replace("()", "").Replace(" ", "");
            //Console.WriteLine($"Scanning {currentReading} for Boss-End function");
        }

        if (c(i, "enemy_delete(") && possibleFuncs.Contains(currentReading) && !bossEndFunctions.Contains(currentReading))
        {
            Console.WriteLine($"Detected Boss-End Function: {currentReading}");
            bossEndFunctions.Add(currentReading);
        }
    }
    */
}

void ScanEnemyFuncs() //checks for every function that is used when spawning an enemy
{
    FuncsFound.Clear();
    for (int i = 0; i < timelinePos; i++) //Scans function names until it reaches the timeline
    {
        if (c(i, "sub ") && e(i, "()")) //start of a function
        {
            FuncsFound.Add(ex(i, 2).Replace("()", ""));
        }
    }

    for (int i = 0; i < eosdSTG.Length; i++) //Scans for enemy_create uses
    {
        if (c(i, "enemy_create") || c(i, "ins_0") || c(i, "ins_2") || c(i, "ins_4") || c(i, "ins_6")) //looks for enemy_create
        {
            for (int j = 0; j < FuncsFound.Count; j++) //Loops through the functions list to find which one it should add
            {
                if (c(i, FuncsFound[j]) && !enemyFunctions.Contains(FuncsFound[j]))
                {
                    Console.WriteLine($"Found function that needs register workaround: {FuncsFound[j]}");
                    enemyFunctions.Add(FuncsFound[j]);
                    break;
                }
            }
        }
    }
}

bool f(string func, string s, ref int line) //Looks on function 'func' for string 's'. stores the line in 'l'
{
    int functionLine = 0;
    for (int i = 0; i < timelinePos; i++)
    {
        if (c(i, "sub ") && e(i, "()") && functionLine == 0) //start of a function
        {
            Console.WriteLine($"FIND: Function [{ex(i, 2).Replace("()", "")}] against [{func}]");
            if (ex(i, 2).Replace("()", "") == func)
            {
                functionLine = i;
            }
        }
        else if (c(i, "}") && functionLine != 0) //end of a function
        {
            return false;
        }
        else if (functionLine != 0)
        {
            if (c(i, s)) //found
            {
                line = i;
                return true;
            }
        }
    }

    return false;
}

//0 = default (none)
//1 = timeout
//2 = life
//3 = death
void SetFuncs(int type)
{
    switch (type)
    {
        case 0:
            b(1, "EI3 = 0;");
            break;

        case 1:
            b(1, "EI3 = EI3 | 1;");
            break;

        case 2:
            b(1, "EI3 = EI3 | 2;");
            break;

        case 3:
            b(1, "EI3 = EI3 | 4;");
            break;
    }
}

//OLD!:
/*void SetFuncs() //sets variables for the life and timeout interrupts
{
    if (!alreadySetFuncs)
    {
        b(1, "int LIFEINT = 0;");
        b(1, "int TIMEINT = 0;");
        alreadySetFuncs = true;
    }
}*/

string GetItem(string s) //Converts EoSD items into FW's and manages some randomness too
{
    switch (s)
    {
        case "-1" or "0": //random between point and power items
            return "(RAND_INT % 2)";
            break;

        case "-2":
            return "0";
            break;
    }
    return s;
}

//unused, somehow i fixed it lmao
void ChangeVarMaps() //Changes stuff like RAND_INT into [-10000] cuz thtk doesn't want to take it for some reason
{
    for (int i = 0; i < fwSTG.Count; i++)
    {
        fwSTG[i] = fwSTG[i].Replace("RAND_INT", "[-10000]").Replace("RAND_FLOAT", "[-9999.0f]")
        .Replace("RAND_ANGLE", "[-9998.0f]").Replace("SELF_X", "[-9997.0f]").Replace("SELF_Y", "[-9996.0f]")
        .Replace("SELF_X_ABS", "[-9995.0f]").Replace("SELF_Y_ABS", "[-9994.0f]")
        .Replace("PLAYER_X", "[-9991.0f]").Replace("PLAYER_Y", "[-9990.0f]").Replace("PLAYER_ANGLE", "[-9989.0f]")
        .Replace("PLAYER_DISTANCE", "[-9944.0f]").Replace("SELF_ANGLE_ABS", "[-9971.0f]").Replace("SELF_SPEED_ABS", "[-9969.0f]");
    }
}

string ConvertSFX(string sfx)
{
    switch (sfx)
    {
        case "6": //evil sealing circle
            return "29";
            break;


        case "8": //bullet shoot (medium)
            return "22";
            break;

        case "9": //bullet shoot (low)
            return "23";
            break;

        case "16": //lasers
            return "18";
            break;

        case "18": //boss defeated
            return "5";
            break;
    }
    return "21"; //bullet default (and ID 7)
}

void CreateLaser(int i, string angle)
{
    b(1, "shooter_reset(1);"); //shooter 1 is for lasers
    switch (ex(i, 2))
    {
        case "0": //simple laser
            b(1, $"bullet_sprite(1, 38, {ex(i, 3)});");
            break;
    }
    b(1, $"bullet_effects_add(1, 0, 7, 999999, -1, -1.0f, -1.0f);");

    b(1, $"shoot_angle(1, {angle}, {angle});");
    b(1, $"bullet_speed(1, {ex(i, 5)}, {ex(i, 5)});");
    if (ex(i, 5) != "0.0f") tempLaserMoves = true;

    b(1, $"laser_size_data(1, {ex(i, 7)}, {ex(i, 8)}, 0.0f, {ex(i, 9)});");
    b(1, $"laser_timing_data(1, {ex(i, 10)}, 30, {ex(i, 11)}, {ex(i, 12)}, 0);");
    if (tempLaserMoves) //if the laser moves
    {
        b(1, $"laser_line_create(1);");
    }
    else
    {
        if (!alreadySetLaser)
        {
            b(1, "int laserID = 0;");
            alreadySetLaser = true;
        }
        if (laserFunctions.Contains(currentFunction))
        {
            LaserRotateFunc("laserID", angle, false);
        }
        b(1, $"laser_infinite_create(1, laserID);");
    }
}


void ApplyBackups() //applies backup registers once it returns from a function
{/*
    if (alreadySetBackup)
    {
        b(1, "$EF0 = BF0;");
        b(1, "$EF1 = BF1;");
        b(1, "$EF2 = BF2;");
        b(1, "$EF3 = BF3;");
        b(1, "%EI0 = BI0;");
        b(1, "%EI1 = BI1;");
        b(1, "%EI2 = BI2;");
        b(1, "%EI3 = BI3;");
        b(1, "$EF4 = BF4;");
        b(1, "$EF5 = BF5;");
        b(1, "$EF6 = BF6;");
        b(1, "$EF7 = BF7;");
    }*/
}

bool AddBackupsAlt(string func)
{
    if (enemyFunctions.Contains(func.Replace("\"", "")))
    {
        VarsStoreWorkaround();
        return true;
    }
    return false;
}

//return 'true' if the backups are needed, otherwise 'false'
bool AddBackups(int line) //adds temporal backup registers when calling a function
{
    if (enemyFunctions.Contains(ex(line, 2).Replace("\"", "")))
    {
        VarsStoreWorkaround();
        return true;
    }
    return false;
    /*b(1, $"{(!alreadySetBackup ? "int" : "")} BF0 = $EF0;");
    b(1, $"{(!alreadySetBackup ? "int" : "")} BF1 = $EF1;");
    b(1, $"{(!alreadySetBackup ? "int" : "")} BF2 = $EF2;");
    b(1, $"{(!alreadySetBackup ? "int" : "")} BF3 = $EF3;");
    b(1, $"{(!alreadySetBackup ? "float" : "")} BI0 = %EI0;");
    b(1, $"{(!alreadySetBackup ? "float" : "")} BI1 = %EI1;");
    b(1, $"{(!alreadySetBackup ? "float" : "")} BI2 = %EI2;");
    b(1, $"{(!alreadySetBackup ? "float" : "")} BI3 = %EI3;");
    b(1, $"{(!alreadySetBackup ? "int" : "")} BF4 = $EF4;");
    b(1, $"{(!alreadySetBackup ? "int" : "")} BF5 = $EF5;");
    b(1, $"{(!alreadySetBackup ? "int" : "")} BF6 = $EF6;");
    b(1, $"{(!alreadySetBackup ? "int" : "")} BF7 = $EF7;");*/
    alreadySetBackup = true;
}

/*void AddComparison() //adds a temporal comparison variable if it wasn't set yet
{
    if (!alreadySetComp)
    {
        alreadySetComp = true;
        b(1, "int COMPARISON = 0;"); //now becomes EI2
    }
}*/

void VarsRetrieveWorkaround() //some workaround to retrieve registers from the enemy's built in variables
{
    //e.g. (float -> int) 3.1415927f becomes 3141592.  (int -> float) 3141592 becomes 3.141592f
    //used on the start of a function when an enemy is spawned

    if (enemyFunctions.Contains(currentFunction))
    {
        b(1, "int I0 = _S(EF0);");
        b(1, "int I1 = _S(EF1);");
        b(1, "int I2 = _S(EF2);");
        b(1, "int I3 = _S(EF3);");

        b(1, "float F0 = _f(EI0) * 0.000001f;");
        b(1, "float F1 = _f(EI1) * 0.000001f;");
        b(1, "float F2 = _f(EI2) * 0.000001f;");
        b(1, "float F3 = _f(EI3) * 0.000001f;");

        b(1, "int IC0 = _S(EF4);");
        b(1, "int IC1 = _S(EF5);");
        b(1, "int IC2 = _S(EF6);");
        b(1, "int IC3 = _S(EF7);");
        b(1, "if (EF6 != 0.0f) EF6 = 0.0f;");
        b(1, "if (EF7 != 0.0f) EF7 = 0.0f;");
    }
}

void VarsStoreWorkaround() //some workaround to store registers into the enemy's built in variables
{
    //use same logic as above but store instead of retrieve.


    //used before spawning enemies

    b(1, "EF0 = _f(I0) + 0.2f;");
    b(1, "EF1 = _f(I1) + 0.2f;");
    b(1, "EF2 = _f(I2) + 0.2f;");
    b(1, "EF3 = _f(I3) + 0.2f;");

    b(1, "EI0 = _S(F0 * 1000000.0f);");
    b(1, "EI1 = _S(F1 * 1000000.0f);");
    b(1, "EI2 = _S(F2 * 1000000.0f);");
    b(1, "EI3 = _S(F3 * 1000000.0f);");

    b(1, "EF4 = _f(IC0) + 0.2f;");
    b(1, "EF5 = _f(IC1) + 0.2f;");
    b(1, "EF6 = _f(IC2) + 0.2f;");
    b(1, "EF7 = _f(IC3) + 0.2f;");
    b(1, "if (EF6 != 0.0f) EF6 = 0.0f;");
    b(1, "if (EF7 != 0.0f) EF7 = 0.0f;");
    //(COMPLETE! )TODO: make a list of the functions that aren't used to spawn anything so it can just do @Function(register1, register2, etc); to make things easier
}

void fixInsVars() //fixes variable names to match FW's
{
    for (int i = 0; i < eosdSTG.Length; i++)
    {
        eosdSTG[i] = convertVar(eosdSTG[i], true);
    }
}

string convertVar(string var, bool extra) //changes the names of the variables to be compatible with FW (if 'extra' is true, it ignores the is-integer check)
{
    if (!int.TryParse(var, out int dummy) || extra)
    {
        return var.Replace("$IC0", "IC0").Replace("$IC1", "IC1").Replace("$IC2", "IC2").Replace("$IC3", "IC3").
        Replace("$F", "F");
        /*Replace("$IC0", "$EF4").Replace("$IC1", "$EF5").Replace("$IC2", "$EF6").Replace("$IC3", "$EF7")
        .Replace("%I", "$EF").Replace("$I", "$EF").Replace("%F", "%EI").Replace("$F", "%EI");*/ //old

    }
    return var;
}

bool isFlagPresent(int input, int flag) //checks if a bitflag is set
{
    return (input & flag) == flag;
}

string convertBullet(string id) //Converts the ID of a bullet from EoSD into FW
{
    switch (id)
    {
        case "1": //outline
            return "6";
            break;

        case "2": //rice
            return "8";
            break;

        case "3": //normal
            return "4";
            break;

        case "4": //kunai
            return "9";
            break;

        case "5": //crystal
            return "9";
            break;

        case "6": //mentos
            return "18";
            break;

        case "7": //fireball
            return "25";
            break;

        case "8": //swords
            return "21";
            break;

        case "9": //bubbles
            return "32";
            break;
    }
    return id; //for ID 0 (Snow)
}


int argsFind(int line) //returns the amount of arguments an instruction has in a line
{
    int tmp = 0;
    exOg(line, ref tmp, true);
    return tmp - 1;
}

string ex(int line, int n)
{
    return exOg(line, ref n, false);
}

string exOg(int line, ref int n, bool find) //removes spaces and extracts strings from an instruction, then returns the string that was found with a 'n' index. if 'find' is true, it returns the amount of arguments found
{
    string builder = "";
    bool onString = false;
    bool escape = false;
    if (eosdSTG[line].Length > 0)
    {
        for (int i = 0; i < eosdSTG[line].Length || (n <= 0 && !find); i++)
        {
            if ((eosdSTG[line][i] == ' ' || eosdSTG[line][i] == ',' || eosdSTG[line][i] == '(' || eosdSTG[line][i] == ')') && !onString)
            {
                if (builder != "")
                {
                    if (find)
                    {
                        n++;
                    }
                    else
                    {
                        n--;
                    }
                    if (n <= 0 && !find)
                    {
                        return builder;
                    }
                    builder = "";
                }
            }
            else if ((eosdSTG[line][i] == ' ' || eosdSTG[line][i] == ',') && onString)
            {
                builder += eosdSTG[line][i];
            }
            else if (eosdSTG[line][i] == '"' && escape)
            {
                builder += eosdSTG[line][i];
                escape = false;

            }
            else if (eosdSTG[line][i] == '"' && !escape)
            {
                builder += eosdSTG[line][i];
                if (onString)
                {
                    onString = false;
                    if (find)
                    {
                        n++;
                    }
                    else
                    {
                        n--;
                    }

                    if (n <= 0 && !find)
                    {
                        return builder;
                    }
                    builder = "";
                }
                else
                {
                    onString = true;
                }
            }
            else if (eosdSTG[line][i] == '\\')
            {
                escape = true;
                builder += eosdSTG[line][i];
            }
            else
            {
                builder += eosdSTG[line][i];
            }
        }

    }
    return builder;
}

bool e(int l, string d) //checks if a line 'l' ends with 'd'
{
    return eosdSTG[l].EndsWith(d);
}

bool s(int l, string z) //checks if a line 'l' starts with 'z'
{
    return eosdSTG[l].StartsWith(z);
}

bool c(int l, string d) //checks if eosd ecl has 'd' on line 'l
{
    return eosdSTG[l].Contains(d);
}

void b(int t, string c) //same as 'a' but it adds 't' tabs before it
{
    string builder = "";
    for (int i = 0; i < t; i++)
    {
        builder += "    ";
    }
    builder += c;
    if (outsideTimeline)
    {
        fwSTG.Insert(fwInsert, builder);
        fwInsert++;
    }
    else
    {
        fwSTG.Add(builder);
    }
}

void a(string b) //its just add smth to fw
{
    if (outsideTimeline)
    {
        fwSTG.Insert(fwInsert, b);
        fwInsert++;
    }
    else
    {
        fwSTG.Add(b);
    }
}

void D([CallerLineNumber] int line = 0) //DEBUG. prints the line of the last function parsed
{
    if (Debugging)
    {
        Console.WriteLine($"Parsed function at line {line}");
    }
}




/*
void Stage2()
{

}

void Stage3()
{

}

void Stage4()
{

}

void Stage5()
{

}

void Stage6()
{

}

void StageEX()
{

}


struct MultiCallback
{
    public string timeoutFunc;
    public string lifeFunc;
}
*/