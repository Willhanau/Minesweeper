#Author: William Hanau
#Date: 11-29-16
#Updated Version(v2.0) of my MineSweeper python game

from graphics import *
import random

TILE_IMAGE = 'tile.gif'
QUESTION_IMAGE = 'question_mark.gif'
FLAG_IMAGE = 'flag.gif'
MINE_IMAGE = 'mine.gif'
LOSE_IMAGE = 'lose.gif'
SMILEY_IMAGE = 'smiley.gif'
SMILEY_POINT = Point(595, 65)
WIDTH_OF_IMAGES = 32
HEIGHT_OF_IMAGES = 32
LEFT_OFFSET = 100
TOP_OFFSET = 120
X_OFFSET = LEFT_OFFSET
Y_OFFSET = TOP_OFFSET
WINDOW_X = 1200
WINDOW_Y = 800
ROWS = 0
COLUMNS = 0
NUM_MINES = 0
NUM_FLAGGED_MINES = 0
DIFF_OFFSET = 0
WIN = GraphWin('MineSweeper' , WINDOW_X , WINDOW_Y, autoflush = False)

class Cell: #acts like C-style struct
    def __init__(self):
        self.matrix_num = None
        self.rectangle = None
        self.tile = None
        self.flag = None

def create_Difficulty_Cells(length):
    difficulty_list = []
    diff_str = ['Beginner', 'Intermediate', 'Expert']
    diff_text_color = ['blue', 'yellow', 'red']
    rect_x = X_OFFSET + 350
    rect_y = X_OFFSET + 100
    text_x = 250 + 350
    text_y = 150 + 100
    for i in range(length):
        c = Cell()
        point1 = Point(rect_x, rect_y)
        point2 = Point(rect_x + 300, rect_y + 100)
        rect = Rectangle(point1, point2)
        rect.setFill('grey')
        rect.draw(WIN)
        c.rectangle = rect

        center_of_text = Point(text_x, text_y)
        text_to_display = Text(center_of_text, diff_str[i])
        text_to_display.setSize(25)
        text_to_display.setTextColor(diff_text_color[i])
        text_to_display.draw(WIN)
        c.mine_or_text = text_to_display

        difficulty_list.append(c)
        rect_y += 105
        text_y += 105
    return difficulty_list

def find_Selected_Difficulty(click_point, difficulty_list):
    cx = click_point.getX()
    cy = click_point.getY()
    for u in range(len(difficulty_list)):
        x1 = difficulty_list[u].rectangle.getP1().x
        y1 = difficulty_list[u].rectangle.getP1().y
        x2 = difficulty_list[u].rectangle.getP2().x
        y2 = difficulty_list[u].rectangle.getP2().y
        if x1 <= cx <= x2 and y1 <= cy <= y2:
            return u
    return None

def pick_Difficulty(idx, difficulty_list):
    global ROWS
    global COLUMNS
    global NUM_MINES
    global DIFF_OFFSET
    if idx == 0:
        ROWS = 9
        COLUMNS = 9
        NUM_MINES = 10
        DIFF_OFFSET = 350
    elif idx == 1:
        ROWS = 16
        COLUMNS = 16
        NUM_MINES = 40
        DIFF_OFFSET = 240
    elif idx == 2:
        ROWS = 16
        COLUMNS = 30
        NUM_MINES = 99
    smiley = Image(SMILEY_POINT, SMILEY_IMAGE)
    smiley.draw(WIN)
    for i in range(len(difficulty_list)):
        difficulty_list[i].rectangle.undraw()
        difficulty_list[i].mine_or_text.undraw()

def create_Board_Text():
    #draw board text
    x = X_OFFSET + 16 + DIFF_OFFSET
    y = Y_OFFSET + 16
    for i in range(COLUMNS):
        center_of_text = Point(x, Y_OFFSET - 15)
        text_to_display = Text(center_of_text, i)
        text_to_display.setSize(15)
        text_to_display.setTextColor('black')
        text_to_display.draw(WIN)
        x += 32
    x = X_OFFSET + DIFF_OFFSET
    for u in range(ROWS):
        center_of_text = Point(x - 15, y)
        text_to_display = Text(center_of_text, u)
        text_to_display.setSize(15)
        text_to_display.setTextColor('black')
        text_to_display.draw(WIN)
        y += 32

def check_Duplicate_Mines(mine_matrix, rand_x, rand_y):
    for j in mine_matrix:
        if j[0] == rand_x and j[1] == rand_y:
            return False
    return True

def create_Mine_Sweeper_Matrix():
    cell_Matrix = []
    #create starting empty cell matrix
    for i in range(ROWS):
        cell_Matrix.append([])
        for j in range(COLUMNS):
            cell_Matrix[i].append(Cell())
            cell_Matrix[i][j].matrix_Num = 0

    #place random mines into cell matrix
    mine_matrix = []
    for i in range(NUM_MINES):
        mine_idx = []
        is_duplicate = False
        while is_duplicate != True:
            rand_x = random.randrange(0, ROWS)
            rand_y = random.randrange(0, COLUMNS)
            is_duplicate = check_Duplicate_Mines(mine_matrix, rand_x, rand_y)
        mine_idx.append(rand_x)
        mine_idx.append(rand_y)
        mine_matrix.append(mine_idx)
    for each in mine_matrix:
        cell_Matrix[each[0]][each[1]].matrix_Num = 13

    #create minesweeper matrix squares, find number of mines around cell, create number of mines around cell text, and place tile images
    y = TOP_OFFSET
    x = LEFT_OFFSET + DIFF_OFFSET
    for i in range(ROWS):
        for j in range(COLUMNS):
            rect_point1 = Point(x, y)
            rect_point2 = Point(x + WIDTH_OF_IMAGES, y + HEIGHT_OF_IMAGES)
            image_point = Point(x + (WIDTH_OF_IMAGES / 2), y + (HEIGHT_OF_IMAGES / 2))
            #create minesweeper matrix squares
            rect = Rectangle(rect_point1, rect_point2)
            rect.draw(WIN)
            cell_Matrix[i][j].rectangle = rect
            if cell_Matrix[i][j].matrix_Num == 13:
                mine = Image(image_point, MINE_IMAGE)
                mine.draw(WIN)
            #find number of mines around cell
            if cell_Matrix[i][j].matrix_Num == 0:
                cell_Matrix[i][j].matrix_Num = mines_Around_Cell(cell_Matrix, i, j)
            #create number of mines around cell text
            if cell_Matrix[i][j].matrix_Num > 0 and cell_Matrix[i][j].matrix_Num != 13:
                text_to_display = Text(image_point, cell_Matrix[i][j].matrix_Num)
                text_to_display.setSize(15)
                text_to_display.setTextColor('blue')
                text_to_display.draw(WIN)
            #place tile images
            tile = Image(image_point, TILE_IMAGE)
            tile.draw(WIN)
            cell_Matrix[i][j].tile = tile
            x += 32
        y += 32
        x = LEFT_OFFSET + DIFF_OFFSET

    #print cell matrix
    for i in range(len(cell_Matrix)):
        print("[", end = "")
        for j in range(len(cell_Matrix[i])):
            if j+1 != len(cell_Matrix[i]):
                print(cell_Matrix[i][j].matrix_Num, end = ", ")
            else:
                print(cell_Matrix[i][j].matrix_Num, end="")
        print("]")
    return cell_Matrix

def mines_Around_Cell(ms_matrix, i, u):
    num = 0
    if ms_matrix[i][u].matrix_Num != 13:
        if i-1 >= 0:
            if ms_matrix[i-1][u].matrix_Num == 13:
                num += 1
        if u-1 >= 0:
            if ms_matrix[i][u-1].matrix_Num == 13:
                num += 1
        if i+1 < len(ms_matrix):
            if ms_matrix[i+1][u].matrix_Num == 13:
                num += 1
        if u+1 < len(ms_matrix[0]):
            if ms_matrix[i][u+1].matrix_Num == 13:
                num += 1
        if i+1 < len(ms_matrix) and u+1 < len(ms_matrix[0]):
            if ms_matrix[i+1][u+1].matrix_Num == 13:
                num += 1
        if i-1 >= 0 and u-1 >= 0:
            if ms_matrix[i-1][u-1].matrix_Num == 13:
                num += 1
        if i+1 < len(ms_matrix) and u-1 >= 0:
            if ms_matrix[i+1][u-1].matrix_Num == 13:
                num += 1
        if i-1 >= 0 and u+1 < len(ms_matrix[0]):
            if ms_matrix[i-1][u+1].matrix_Num == 13:
                num += 1
        return num
    return 0

def find_Selected_Square(click_point, cell_Matrix):
    cx = click_point.getX()
    cy = click_point.getY()
    for i in range(len(cell_Matrix)):
        for u in range(len(cell_Matrix[i])):
            x1 = cell_Matrix[i][u].rectangle.getP1().x
            y1 = cell_Matrix[i][u].rectangle.getP1().y
            x2 = cell_Matrix[i][u].rectangle.getP2().x
            y2 = cell_Matrix[i][u].rectangle.getP2().y
            if x1 <= cx <= x2 and y1 <= cy <= y2:
                matrix_idx = []
                matrix_idx.append(i)
                matrix_idx.append(u)
                return matrix_idx
    return None

def expose_Cell(idx, cell_Matrix):
    if cell_Matrix[idx[0]][idx[1]].flag == None:
        if cell_Matrix[idx[0]][idx[1]].matrix_Num == 13:
            expose_All_Mines(cell_Matrix)
            return True
        else:
            expose_Surrounding_Cells(idx, cell_Matrix)
    return False

def expose_All_Mines(cell_Matrix):
    for each in cell_Matrix:
        for i in each:
            if i.matrix_Num == 13:
                i.tile.undraw()
                if i.flag != None:
                    i.flag.undraw()
                    line1 = Line(i.rectangle.p1, i.rectangle.p2)
                    line1.setWidth(5)
                    line1.draw(WIN)
    lose = Image(SMILEY_POINT, LOSE_IMAGE)
    lose.draw(WIN)
    lose_text1 = Text(Point(SMILEY_POINT.getX() - 150, SMILEY_POINT.getY()), "You LOSE :(")
    lose_text1.setSize(25)
    lose_text1.draw(WIN)
    lose_text2 = Text(Point(SMILEY_POINT.getX() + 150, SMILEY_POINT.getY()), "You LOSE :(")
    lose_text2.setSize(25)
    lose_text2.draw(WIN)

def expose_Surrounding_Cells(idx, cell_Matrix): #recursive method
    if idx[0] >= len(cell_Matrix) or idx[1] >= len(cell_Matrix[0]) or idx[0] < 0 or idx[1] < 0:
        return
    if cell_Matrix[idx[0]][idx[1]].flag != None or cell_Matrix[idx[0]][idx[1]].matrix_Num == 13 or cell_Matrix[idx[0]][idx[1]].matrix_Num == -1:
        return
    temp = [0,0]
    cell_Matrix[idx[0]][idx[1]].tile.undraw()
    if cell_Matrix[idx[0]][idx[1]].matrix_Num > 0:
        cell_Matrix[idx[0]][idx[1]].matrix_Num = -1
        return
    else:
        cell_Matrix[idx[0]][idx[1]].matrix_Num = -1
        temp[0] = idx[0]
        temp[1] = idx[1]+1
        expose_Surrounding_Cells(temp, cell_Matrix)

        temp[0] = idx[0]+1
        temp[1] = idx[1]
        expose_Surrounding_Cells(temp, cell_Matrix)

        temp[0] = idx[0]+1
        temp[1] = idx[1]+1
        expose_Surrounding_Cells(temp, cell_Matrix)

        temp[0] = idx[0]
        temp[1] = idx[1]-1
        expose_Surrounding_Cells(temp, cell_Matrix)

        temp[0] = idx[0]-1
        temp[1] = idx[1]
        expose_Surrounding_Cells(temp, cell_Matrix)

        temp[0] = idx[0]-1
        temp[1] = idx[1]-1
        expose_Surrounding_Cells(temp, cell_Matrix)

        temp[0] = idx[0]+1
        temp[1] = idx[1]-1
        expose_Surrounding_Cells(temp, cell_Matrix)

        temp[0] = idx[0]-1
        temp[1] = idx[1]+1
        expose_Surrounding_Cells(temp, cell_Matrix)
    return

def placeFlag_OnCell(idx, cell_Matrix):
    global NUM_FLAGGED_MINES
    if cell_Matrix[idx[0]][idx[1]].flag == None:
        point = Point(cell_Matrix[idx[0]][idx[1]].rectangle.getP1().x + 16, cell_Matrix[idx[0]][idx[1]].rectangle.getP1().y + 16)
        flag = Image(point, FLAG_IMAGE)
        flag.draw(WIN)
        cell_Matrix[idx[0]][idx[1]].flag = flag
        NUM_FLAGGED_MINES += 1
    else:
        cell_Matrix[idx[0]][idx[1]].flag.undraw()
        cell_Matrix[idx[0]][idx[1]].flag = None
        NUM_FLAGGED_MINES -= 1
    return check_Game_Win(cell_Matrix)

def check_Game_Win(cell_Matrix):
    if NUM_FLAGGED_MINES != NUM_MINES:
        return False
    sum = 0
    for i in range(len(cell_Matrix)):
        for j in range(len(cell_Matrix[i])):
            if cell_Matrix[i][j].flag != None and cell_Matrix[i][j].matrix_Num == 13:
                sum += 1
    if sum == NUM_MINES:
        win_text1 = Text(Point(SMILEY_POINT.getX() - 125, SMILEY_POINT.getY()), "You WIN!")
        win_text1.setSize(25)
        win_text1.draw(WIN)
        win_text2 = Text(Point(SMILEY_POINT.getX() + 125, SMILEY_POINT.getY()), "You WIN!")
        win_text2.setSize(25)
        win_text2.draw(WIN)
        return True
    else:
        return False

def start():
    difficulty = False
    difficulty_list = create_Difficulty_Cells(3)
    while difficulty != True:
        click_point = WIN.getMouse()
        idx = find_Selected_Difficulty(click_point, difficulty_list)
        if idx != None:
            pick_Difficulty(idx, difficulty_list)
            difficulty = True
    create_Board_Text()
    cell_Matrix = create_Mine_Sweeper_Matrix()
    return cell_Matrix

def update_frame(cell_Matrix):
    click_point = WIN.getMouse()
    idx = find_Selected_Square(click_point, cell_Matrix)
    if idx != None:
        if WIN.lastButton == "Left Click" and cell_Matrix[idx[0]][idx[1]].matrix_Num != -1:
            return expose_Cell(idx, cell_Matrix)
        elif cell_Matrix[idx[0]][idx[1]].matrix_Num != -1:
            return placeFlag_OnCell(idx, cell_Matrix)
    return False

def main():
    game = False
    cell_Matrix = start()
    while game != True:
        game = update_frame(cell_Matrix)
    WIN.getMouse()
    WIN.close()
main()