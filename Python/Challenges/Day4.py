import sys
import re as regex
from typing import List, Dict
from datetime import datetime

# Type aliases
Timesheet = List[int]


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Setup parsing info
    timestamps: Dict[datetime, str] = {}
    splitter: regex = regex.compile(r".+(\d{2}-\d{2} \d{2}:\d{2})[^#]+#?(wakes|falls|\d+).+")  # What have I done?

    # Annotate tuple unpacking variables
    time: str
    op: str

    # Parse info from the file
    with open(args[1], "r") as f:
        for line in f:
            time, message = splitter.search(line).groups()
            timestamps[datetime.strptime(time, "%m-%d %H:%M")] = message

    # Setup lookup variables
    schedules: Dict[int, Timesheet] = {}
    start: int
    timesheet: Timesheet
    timestamp: datetime
    guard: int
    # Loop through timestamps in chronological order
    for timestamp, op in sorted(timestamps.items(), key=lambda t: t[0]):
        # On fall asleep mark time
        if op == "falls":
            start = timestamp.minute

        # On wake up add to sleeping timesheet
        elif op == "wakes":
            for i in range(start, timestamp.minute):
                timesheet[i] += 1

        # Get sleep schedule
        else:
            guard = int(op)
            if op not in schedules:
                timesheet = [0] * 60
                schedules[guard] = timesheet
            else:
                timesheet = schedules[guard]

    # Best candidate setup
    best: int = 0
    maximum: int = 0
    for guard in schedules:
        # Get maximum sleep time
        sleep: int = sum(schedules[guard])
        if sleep > maximum:
            maximum = sleep
            best = guard

    # Print result
    timesheet = schedules[best]
    minute: int = timesheet.index(max(timesheet))
    print(f"Part one hash: {best * minute}")

    # Best candidate setup again
    best = 0
    maximum = 0
    for guard in schedules:
        # Get most frequent sleep time
        frequent: int = max(schedules[guard])
        if frequent > maximum:
            maximum = frequent
            best = guard

    # Print result
    timesheet = schedules[best]
    minute = timesheet.index(maximum)
    print(f"Part two hash: {best * minute}")


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
