import sys
import re
from datetime import datetime


def main(args):
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Setup parsing info
    timestamps = []
    splitter = re.compile(r".+(\d{2}-\d{2} \d{2}:\d{2})[^#]+#?(wakes|falls|\d+).+")  # How have I come up with this

    # Parse info from the file
    with open(args[1], "r") as f:
        for line in f:
            date, op = splitter.search(line).groups()
            timestamps.append((datetime.strptime(date, "%m-%d %H:%M"), op))

    # Setup lookup variables
    schedules = {}
    timesheet = None
    start = 0
    # Loop through timestamps in chronological order
    for timestamp, op in sorted(timestamps, key=lambda t: t[0]):
        # On fall asleep mark time
        if op == "falls":
            start = timestamp.minute
        # On wake up add to sleeping timesheet
        elif op == "wakes":
            for i in range(start, timestamp.minute):
                timesheet[i] += 1
        # Get sleep schedule
        else:
            op = int(op)
            if op not in schedules:
                timesheet = [0] * 60
                schedules[op] = timesheet
            else:
                timesheet = schedules[op]

    # Best candidate setup
    best = 0
    maximum = 0
    for ID in schedules:
        # Get maximum sleep time
        sleep = sum(schedules[ID])
        if sleep > maximum:
            maximum = sleep
            best = ID

    # Print result
    timesheet = schedules[best]
    minute = timesheet.index(max(timesheet))
    print(f"Part one hash: {best * minute}")

    # Best candidate setup again
    best = 0
    maximum = 0
    for ID in schedules:
        # Get most frequent sleep time
        frequent = max(schedules[ID])
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
