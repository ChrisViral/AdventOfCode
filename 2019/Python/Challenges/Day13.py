import sys
from tkinter import Tk, Label, StringVar
from typing import List, Tuple, Final
from utils import IntcodeComp, Grid, Vector
from enum import IntEnum
from threading import Thread


class Block(IntEnum):
    """
    Block types enum
    """
    EMPTY = 0
    WALL = 1
    BLOCK = 2
    PADDLE = 3
    BALL = 4

    def __str__(self) -> str:
        """
        Gets the visual representation of the blocks
        :return: The string visual equivalent of the given block
        """
        return Block._as_str[self]


# Block type to string value list, correctly indexed
Block._as_str = [" ", "▓", "▒", "—", "o"]

# Position indicating it is writing a score and not a position on the game grid
score_pos: Final[Vector] = Vector(-1, 0)


def play(window: Tk, text: StringVar, score: StringVar, comp: IntcodeComp, grid: Grid[Block], cycles: int = 0) -> None:
    """
    Plays a frame of the Block Game in the Intcode VM
    :param window: Game window
    :param text: Grid text
    :param score: Score label
    :param comp: Intcode VM running the game
    :param grid: Game grid
    :param cycles: Empty cycles ran
    """

    # Check if there is anything new to display
    if len(comp.output_buffer) == 0:
        # If nothing new has been available to display for five frames, assume Game over
        if cycles == 5:
            # Print score and return
            print(score.get())
            return
        else:
            # Wait until next frame, and increment inactivity counter
            window.after(33, lambda: play(window, text, score, comp, grid, cycles + 1))

    # Position of the paddle and ball
    paddle: Vector = Vector(0, 0)
    ball: Vector = Vector(0, 0)

    # Pop out all the info in the output buffer
    while len(comp.output_buffer) >= 3:
        # Get position to write to
        pos: Vector = Vector(comp.next_output(), comp.next_output())
        # If score position, update score
        if pos == score_pos:
            score.set(comp.next_output())
        else:
            # Else get block type, and keep track if it's the paddle or ball
            block: Block = Block(comp.next_output())
            if block == Block.PADDLE:
                paddle = pos
            elif block == Block.BALL:
                ball = pos
            # Update the grid
            grid[pos] = block

    # Update the game label
    text.set(str(grid))
    # Input the new position of the paddle
    if ball.x != paddle.x:
        comp.add_input(-1 if ball.x < paddle.x else 1)
    else:
        comp.add_input(0)

    # Schedule the next update
    window.after(33, lambda: play(window, text, score, comp, grid))


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # File read stub
    with open(args[1], "r") as f:
        line = f.readline().strip()
        comp: IntcodeComp = IntcodeComp(line)

    # Count the amount of blocks
    blocks: int = 0
    size: Tuple[int, int] = 0, 0
    comp.run_program()
    while len(comp.output_buffer) >= 3:
        size = max(size, (comp.next_output(), comp.next_output()))
        blocks += 1 if comp.next_output() == 2 else 0
    print(blocks)

    # Setup the Intcode VM
    comp = IntcodeComp("2" + line[1:], threaded=True)
    thread: Thread = Thread(target=comp.run_program)
    thread.start()
    # Setup the play window
    game: Tk = Tk(screenName="Game")
    game.title("Block Game")
    # Setup the labels
    text: StringVar = StringVar()
    text.set("\n")
    Label(game, textvariable=text, font="Consolas").pack()
    score: StringVar = StringVar()
    score.set("Score: 0")
    Label(game, textvariable=score, font="Consolas").pack()
    # Setup the game grid
    grid: Grid[Block] = Grid(size[0] + 1, size[1] + 1)
    # Setup the update loop
    game.after(33, lambda: play(game, text, score, comp, grid))
    # Run the game
    game.mainloop()
    thread.join()


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
