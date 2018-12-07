import sys
import re
from typing import List, Dict
from datetime import datetime


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Setup parsing info
    timestamps: Dict[datetime, str] = {}
    splitter: re = re.compile(r".+(\d{2}-\d{2} \d{2}:\d{2})[^#]+#?(wakes|falls|\d+).+")  # How have I come up with this

    # Annotate tuple unpacking variables
    timestamp: datetime
    op: str

    # Parse info from the file
    with open(args[1], "r") as f:
        for line in f:
            timestamp, op = splitter.search(line).groups()
            timestamps[datetime.strptime(timestamp, "%m-%d %H:%M")] = op

    # Setup lookup variables
    schedules: Dict[int, List[int]] = {}
    timesheet: List[int] = None
    start: int = 0
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
            op: int = int(op)
            if op not in schedules:
                timesheet = [0] * 60
                schedules[op] = timesheet
            else:
                timesheet = schedules[op]

    # Best candidate setup
    best: int = 0
    maximum: int = 0
    ID: int
    for ID in schedules:
        # Get maximum sleep time
        sleep: int = sum(schedules[ID])
        if sleep > maximum:
            maximum = sleep
            best = ID

    # Print result
    timesheet = schedules[best]
    minute: int = timesheet.index(max(timesheet))
    print(f"Part one hash: {best * minute}")

    # Best candidate setup again
    best = 0
    maximum = 0
    for ID in schedules:
        # Get most frequent sleep time
        frequent: int = max(schedules[ID])
        if frequent > maximum:
            maximum = frequent
            best = ID

    # Print result
    timesheet = schedules[best]
    minute = timesheet.index(maximum)
    print(f"Part two hash: {best * minute}")


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
