// TAR 2.0 - Tactical Robot Software
// http://web.tiscalinet.it/ferrarafrancesco/qc/tar/
//
// History:
// 2.0
//   Alpha-Beta pruning
//   More speed
//   Support for some key command:
//     PGRM  quit/reset game (after that key press ONOFF to exit or other key to begin a new game)
//     ONOFF Switch OFF the RCX
//     VIEW  do a setup light 
//     RUN   pass
//   You can now switch Off the RCX playing a game RESUMING from the point of interrupt
//   Dynamic Position Evalutator (It's now hard to beat him)
//   Move Randomizer if found more bestmove
//
// 1.0
//   First Initial Release

#include <qc.h>

#define MOTORH 		OUT_A
#define DISPENSER 	OUT_B
#define MOTORV		OUT_C

#define UP 		REV
#define DOWN 		FWD

#define TCARRIAGE	SENSOR_2
#define EYE		SENSOR_1

#define ONWHITE 100
#define ONBLACK 200

#define SPEEDH 7
#define SPEEDV 7

#define DELTA 8

static int currentx = -1;
static int currenty = -1;
static int basedarkh = 787;
static int basedarkv = 780;
static int minblank = 720, maxblank = 827;

/******************************** BRAIN ************************************************/

#define DIMX 5
#define DIMY 6

#define P0 -1
#define P1 1
#define PN 0

#define NOTSET -1

#define abs(A) ((A)>0?(A):-(A))

int maxlevel = 4;
int basey[DIMX];
int board[DIMX][DIMY];
int pos[DIMX][DIMY] =
{
    {3, 4, 6, 6, 4, 3},
    {4, 7, 9, 9, 7, 4},
    {6, 9, 12, 12, 9, 6},
    {4, 7, 9, 9, 7, 4},
    {3, 4, 6, 6, 4, 3}
};

void calcPos(void)
{
    int x, y;

    for (y = 0; y < DIMY; y++)
	for (x = 0; x < DIMX; x++)
	    pos[x][y] = 0;

    // Horz
    for (y = 0; y < DIMY; y++)
	for (x = 0; x < DIMX - 2; x++) {
	    int a = board[x + 0][y + 0];
	    int b = board[x + 1][y + 0];
	    int c = board[x + 2][y + 0];

	    if (abs(a) + abs(b) + abs(c) == abs(a + b + c))	// tris possible
	     {
		pos[x + 0][y + 0]++;
		pos[x + 1][y + 0]++;
		pos[x + 2][y + 0]++;
	    }
	}

    // Vert
    for (y = 0; y < DIMY - 2; y++)
	for (x = 0; x < DIMX; x++) {
	    int a = board[x + 0][y + 0];
	    int b = board[x + 0][y + 1];
	    int c = board[x + 0][y + 2];

	    if (abs(a) + abs(b) + abs(c) == abs(a + b + c))	// tris possible
	     {
		pos[x + 0][y + 0]++;
		pos[x + 0][y + 1]++;
		pos[x + 0][y + 2]++;
	    }
	}

    // Diag SX/DX
    for (y = 0; y < DIMY - 2; y++)
	for (x = 0; x < DIMX - 2; x++) {
	    int a = board[x + 0][y + 0];
	    int b = board[x + 1][y + 1];
	    int c = board[x + 2][y + 2];

	    if (abs(a) + abs(b) + abs(c) == abs(a + b + c))	// tris possible
	     {
		pos[x + 0][y + 0]++;
		pos[x + 1][y + 1]++;
		pos[x + 2][y + 2]++;
	    }
	}

    // Diag DX/SX
    for (y = 0; y < DIMY - 2; y++)
	for (x = 0; x < DIMX - 2; x++) {
	    int a = board[x + 2][y + 0];
	    int b = board[x + 1][y + 1];
	    int c = board[x + 0][y + 2];

	    if (abs(a) + abs(b) + abs(c) == abs(a + b + c))	// tris possible
	     {
		pos[x + 2][y + 0]++;
		pos[x + 1][y + 1]++;
		pos[x + 0][y + 2]++;
	    }
	}
}

int checkTris(void)
{
    int x, y, tris = 0;

    // Horz
    for (y = 0; y < DIMY; y++)
	for (x = 0; x < DIMX - 2; x++)
	    if (board[x + 0][y] == board[x + 1][y] &&
		board[x + 1][y] == board[x + 2][y] &&
		board[x][y] != PN)
		tris += board[x][y];
    // Vert
    for (y = 0; y < DIMY - 2; y++)
	for (x = 0; x < DIMX; x++)
	    if (board[x][y + 0] == board[x][y + 1] &&
		board[x][y + 1] == board[x][y + 2] &&
		board[x][y] != PN)
		tris += board[x][y];
    // Diag SX/DX
    for (y = 0; y < DIMY - 2; y++)
	for (x = 0; x < DIMX - 2; x++)
	    if (board[x + 0][y + 0] == board[x + 1][y + 1] &&
		board[x + 1][y + 1] == board[x + 2][y + 2] &&
		board[x][y] != PN)
		tris += board[x][y];
    // Diag DX/SX
    for (y = 0; y < DIMY - 2; y++)
	for (x = 0; x < DIMX - 2; x++)
	    if (board[x + 2][y + 0] == board[x + 1][y + 1] &&
		board[x + 1][y + 1] == board[x + 0][y + 2] &&
		board[x + 2][y] != PN)
		tris += board[x + 2][y];

    return tris;
}

int score(void)
{
    int x, y, score = checkTris() * 100;

    calcPos();
    for (y = 0; y < DIMY; y++)
	for (x = 0; x < DIMX; x++)
	    score += board[x][y] * pos[x][y];

    return score;
}

void makeMove(int x, int player)
{
    board[x][--basey[x]] = player;
}

void removeMove(int x)
{
    board[x][basey[x]++] = PN;
}

void searchMove(int *pbestmove, int *pbestscore, int player, int level, int cutoff)
{
    int x, bestscore;

    *pbestmove = NOTSET, *pbestscore = NOTSET;

    for (x = 0; x < DIMX; x++) {
	if (basey[x] > 0) {
	    int dummy;

/***********************************************************************/
	    makeMove(x, player);
	    if (level == maxlevel)
		bestscore = score();
	    else
		searchMove(&dummy, &bestscore, -player, level + 1, *pbestscore);
	    removeMove(x);

/***********************************************************************/
	    if ((player == P0 && (bestscore < *pbestscore)) ||	/* MIN */
		(player == P1 && (bestscore > *pbestscore)) ||	/* MAX */
		((rand() & 1) && (bestscore == *pbestscore)) ||
		*pbestmove == NOTSET) {

		*pbestmove = x;
		*pbestscore = bestscore;

	    }
/************************************************************************/
	    if (player == P0 && cutoff != NOTSET && cutoff > *pbestscore)
		break;
	    if (player == P1 && cutoff != NOTSET && cutoff < *pbestscore)
		break;

	}
	if (level == 0) {
	    LCDPrintn(*pbestscore);
	    PlaySound(0);
	}
    }
}

void initBoard(void)
{
    int x, y;

    for (y = 0; y < DIMY; y++)
	for (x = 0; x < DIMX; x++)
	    board[x][y] = PN;
    for (x = 0; x < DIMX; x++)
	basey[x] = DIMY;
}

int endGame(void)
{
    int endgame = 1, x;

    for (x = 0; x < DIMX; x++) {
	if (basey[x] > 0)
	    endgame = 0;
    }
    return endgame;
}
/******************************** BRAIN ************************************************/


void correct(int dir)
{
    MotorSet(7, dir, MOTORV);
    Wait(42);
    MotorSet(7, BRK, MOTORV);
    Wait(85);
}

void correctPiece(int dir)
{
    MotorSet(7, dir, MOTORV);
    Wait(30);
    MotorSet(7, BRK, MOTORV);
    Wait(85);

}

void putPiece()
{
    correctPiece(UP);

    MotorSet(7, FWD, DISPENSER);
    Wait(200);
    MotorSet(7, REV, DISPENSER);
    Wait(210);
    MotorSet(7, BRK, DISPENSER);

    correctPiece(DOWN);
}

void initCarriage()
{
    MotorSet(SPEEDH, FWD, MOTORH);
    while (!SensorBoolean(TCARRIAGE));
    MotorSet(7, BRK, MOTORH);
    currentx = -1;
}

void moveTo(int x, int y)
{
    static int state = ONWHITE;
    int diff;

    if (currenty != y)
	initCarriage();
/*****************************************************/
    if (currenty < y) {
	MotorSet(SPEEDV, DOWN, MOTORV);
	while (currenty < y) {
	    diff = SensorRaw(EYE) - basedarkv;
	    if (state == ONWHITE && diff > DELTA) {
		state = ONBLACK;
		PlayTone(2000, 10);
		currenty++;
		//LCDPrintn(diff);
	    }
	    if (state == ONBLACK && diff < -DELTA) {
		state = ONWHITE;
		PlayTone(440, 10);
	    }
	}
	//Wait(4);
	MotorSet(7, BRK, MOTORV);
    } else if (currenty > y) {
	MotorSet(SPEEDV, UP, MOTORV);
	while (currenty > y) {
	    diff = SensorRaw(EYE) - basedarkv;
	    if (state == ONWHITE && diff > DELTA) {
		state = ONBLACK;
		PlayTone(2000, 10);
		currenty--;
		//LCDPrintn(diff);
	    }
	    if (state == ONBLACK && diff < -DELTA) {
		state = ONWHITE;
		PlayTone(440, 10);
	    }
	}
	//Wait(4);
	MotorSet(7, BRK, MOTORV);
    }
/*****************************************************/
    if (currentx < x) {
	MotorSet(SPEEDH, REV, MOTORH);
	Wait(20);
	while (currentx < x) {
	    diff = SensorRaw(EYE) - basedarkh;
	    if (state == ONWHITE && diff > DELTA) {
		state = ONBLACK;
		PlayTone(2000, 10);
		currentx++;
		//LCDPrintn(diff);
	    }
	    if (state == ONBLACK && diff < -DELTA) {
		state = ONWHITE;
		PlayTone(440, 10);
	    }
	}
    } else if (currentx > x) {
	MotorSet(SPEEDH, FWD, MOTORH);
	while (currentx > x) {
	    diff = SensorRaw(EYE) - basedarkh;
	    if (state == ONWHITE && diff > DELTA) {
		state = ONBLACK;
		PlayTone(2000, 10);
		currentx--;
		//LCDPrintn(diff);
	    }
	    if (state == ONBLACK && diff < -DELTA) {
		state = ONWHITE;
		PlayTone(440, 10);
	    }
	}
    }
    MotorSet(7, BRK, MOTORH);
}

void goOut(void)
{
    moveTo(-1, 0);
    MotorSet(7, UP, MOTORV);
    Wait(300);
    MotorSet(7, BRK, MOTORV);
    currenty = -1;
}

void setupLight(void)
{
    int dark, max, min;

    currentx = -1;
    currenty = -1;


    SensorActive(EYE);
    initCarriage();

    MotorSet(7, DOWN, MOTORV);
    Wait(450);

    // Vert
    max = 0, min = 2000;
    HiTimerSet(0, 200);
    MotorSet(SPEEDV, UP, MOTORV);
    while (HiTimerGet(0) > 0) {
	dark = SensorRaw(EYE);
	if (dark > max)
	    max = dark;
	if (dark < min)
	    min = dark;
    }
    MotorSet(7, BRK, MOTORV);
    basedarkv = (max + min) / 2;
    LCDPrintn(basedarkv);

    moveTo(-1, 0);

    // Horz
    MotorSet(SPEEDH, REV, MOTORH);
    Wait(150);
    max = 0, min = 2000;
    MotorSet(SPEEDH, FWD, MOTORH);
    while (!SensorBoolean(TCARRIAGE)) {
	dark = SensorRaw(EYE);
	if (dark > max)
	    max = dark;
	if (dark < min)
	    min = dark;
    }
    MotorSet(7, BRK, MOTORH);
    basedarkh = (max + min) / 2;
    LCDPrintn(basedarkh);

    maxblank = 0;
    minblank = 2000;

    // Blank line 0
    moveTo(0, 0);
    correct(UP);
    MotorSet(SPEEDH, REV, MOTORH);
    HiTimerSet(0, 100);
    while (HiTimerGet(0) > 0) {
	dark = SensorRaw(EYE);
	if (dark > maxblank)
	    maxblank = dark;
	if (dark < minblank)
	    minblank = dark;
    }
    MotorSet(SPEEDH, BRK, MOTORH);
    correct(DOWN);

    // Blank line 5
    moveTo(0, 5);
    correct(UP);
    MotorSet(SPEEDH, REV, MOTORH);
    HiTimerSet(0, 100);
    while (HiTimerGet(0) > 0) {
	dark = SensorRaw(EYE);
	if (dark > maxblank)
	    maxblank = dark;
	if (dark < minblank)
	    minblank = dark;
    }
    MotorSet(SPEEDH, BRK, MOTORH);
    correct(DOWN);

    maxblank += 40;
    minblank -= 5;

    LCDPrintn(maxblank);

    goOut();
    SensorActive(EYE);

}

int scanBoard()
{
    int x, y, dark, ret = -1;
    int found = 0;

    for (x = 0; x < DIMX && !found; x++) {
	y = basey[x] - 1;
	if (y >= 0)
	    if (board[x][y] == PN) {
		moveTo(x, y);
		correct(UP);
		dark = SensorRaw(EYE);
		if (dark > maxblank) {
		    ret = x;
		    found = 1;
		    /*
		       LCDPrintn(dark - maxblank);

		       } else if (dark < minblank) {
		       ret = x;
		       found = 1; */
		}
		correct(DOWN);
	    }
    }
    return ret;
}

void displayResult(void)
{
    static int oldtris = 0;

    if (oldtris > checkTris())
	PlaySound(SOUND_LOW_BEEP);
    else if (oldtris < checkTris())
	PlaySound(SOUND_FAST_UP);
    oldtris = checkTris();
    LCDPrintn(checkTris());
}

int main()
{
    int bestmove, bestscore, c, quit;

    LCDPrintn(TimerGet(0));
    srand(TimerGet(0));

    SetSensor(EYE, SENSOR_TYPE_LIGHT, SENSOR_MODE_RAW);
    //SensorActive(EYE);

    SetSensor(TCARRIAGE, SENSOR_TYPE_TOUCH, SENSOR_MODE_BOOL);
    SensorPassive(TCARRIAGE);

    do {
	PlaySound(0);
	quit = 0;
	initBoard();
	while (!quit && !endGame()) {
	    c = getc();
	    if (c == PRGM)
		quit = 1;
	    else if (c == VIEW)
		setupLight();
	    else if (c == ONOFF) {
		PowerOff();
		displayResult();
	    } else {
		SensorActive(EYE);
		LCDRun();

		bestmove = scanBoard();
		if (bestmove >= 0) {
		    PlayTone(2500, 20);
		    PlayTone(1500, 10);
		    PlayTone(3500, 10);

		    makeMove(bestmove, P0);
		    displayResult();
		}
		if (!endGame()) {
		    searchMove(&bestmove, &bestscore, P1, 0, NOTSET);
		    moveTo(bestmove, basey[bestmove] - 1);
		    putPiece();

		    makeMove(bestmove, P1);
		    displayResult();

		}
		goOut();
		PlaySound(0);

		SensorPassive(EYE);
		LCDStand();

	    }
	}
	PlaySound(2);
    } while (getc() != ONOFF);
    return 0;
}
