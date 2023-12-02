import sys
from typing import List, Deque, Optional
from itertools import permutations
from utils import IntcodeComp
from threading import Thread


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # File read stub
    with open(args[1], "r") as f:
        # Save the program code
        line: str = f.readline().strip()

    # Create the Intcode Computer, and link the input and output buffers
    comp: IntcodeComp = IntcodeComp(line)
    comp.input_buffer = comp.output_buffer

    thrust: int = 0
    # For all phase permutations
    for perm in permutations([0, 1, 2, 3, 4]):
        out: Optional[Deque[int]] = None
        # Add original input
        comp.input_buffer.append(0)
        # For each amplifier
        for phase in perm:
            # Append phase and run
            comp.input_buffer.appendleft(phase)
            out = comp.run_program().output

        # Final thrust is in the output buffer
        thrust = max(thrust, out.pop())

    # Print the max thrust
    print(thrust)

    # Reset thrust and create the five amplifiers
    thrust = 0
    amplifiers: List[IntcodeComp] = [IntcodeComp(line, threaded=True) for _ in range(5)]
    # Link all input and output buffers circularly
    for i in range(5):
        amplifiers[i].input_buffer = amplifiers[i - 1].output_buffer

    # For all phase permutations
    for perm in permutations([5, 6, 7, 8, 9]):
        # Input the phase number
        for i in range(5):
            amplifiers[i].add_input(perm[i])

        # Add original input to the first
        amplifiers[0].add_input(0)

        # Create all the amplifier threads
        threads: List[Thread] = [Thread(target=amplifier.run_program) for amplifier in amplifiers]

        # Start all threads, then wait for all threads
        for thread in threads:
            thread.start()
        for thread in threads:
            thread.join()

        # Final thrust is in the last amplifier's output buffer
        thrust = max(thrust, amplifiers[-1].next_output())

    # Print the max thrust
    print(thrust)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
