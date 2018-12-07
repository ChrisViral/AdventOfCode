import sys
import re as regex
from collections import OrderedDict
from typing import List, Dict, Set, Optional


class Task:
    """
    Task object, holds a reference to required task to proceed, as well as tasks unlocked by this
    """

    def __init__(self, task: str) -> None:
        """
        Creates a new Step with the given key
        :param task: Step key identifier
        """

        self.key: str = task
        self.requirements: Set[Task] = set()
        self.unlocks: Set[Task] = set()

    def add_requirement(self, required: "Task") -> None:
        """
        Adds the given task to be required by this task
        This also sets the required task to have this task as an unlockable
        :param required: Required task for this task
        """

        # Add required to requirement, then self to unlocks of requirement
        self.requirements.add(required)
        required.unlocks.add(self)

    def complete(self) -> None:
        """
        Completes this step and removes it as a requirement for children steps
        """
        # Loop through all the steps they unlock
        for unlock in self.unlocks:
            # Remove that requirement from the instruction
            unlock.requirements.remove(self)


class Worker:
    """
    Worker object, holds a task and time left to complete it
    """

    def __init__(self):
        """
        Creates a new worker without any task
        """

        self.task: Optional[Task] = None
        self.time: int = 0

    def assign(self, task: Optional[Task]) -> bool:
        """
        Assigns a task to the given worker if possible.
        The task is not assigned if it is equal to None or if a task is already assigned
        :param task: Task to assign
        :return: True if the task was assigned, False otherwise
        """

        # Only assign if no task is currently assigned and if the new task is not None
        if not self.task and task:
            self.task = task
            self.time = 61 + (ord(task.key) - ord("A"))
            return True
        return False

    def update(self) -> bool:
        """
        Updates the worker and tries to complete the task
        :return: True if the assigned task was completed this turn, false otherwise
        """

        # If a task is assigned
        if self.task:
            # Decrease counter
            self.time -= 1
            # If not time is left, remove task
            if self.time == 0:
                self.task.complete()
                self.task = None
                return True
            # If task not complete, return False
            return False
        # If no task ongoing, return True
        return True


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Setup data structures
    letters: str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    tasks: Dict[str, Task] = OrderedDict([(key, Task(key)) for key in letters])
    pattern: regex = regex.compile("Step ([A-Z]) must be finished before step ([A-Z]) can begin.")
    requirement: Task
    required: Task

    # Read from file
    with open(args[1], "r") as f:
        for line in f:
            # Get tasks and set requirements
            requirement, required = map(tasks.__getitem__, pattern.search(line).groups())
            required.add_requirement(requirement)

    # Setup final sequence
    sequence: str = ""
    # While instructions aren't empty
    while tasks:
        # Loop through all the instructions
        for task in tasks.values():
            # If they have no requirements left
            if not task.requirements:
                # Remove locks from this task
                task.complete()

                # Delete that instruction from the set of instructions to run, and add it to the sequence
                del tasks[task.key]
                sequence += task.key
                break

    print("Part one sequence:", sequence)

    # Recreate tasks structure
    tasks = OrderedDict([(key, Task(key)) for key in letters])
    # Read from file
    with open(args[1], "r") as f:
        for line in f:
            # Get tasks and set requirements
            requirement, required = map(tasks.__getitem__, pattern.search(line).groups())
            required.add_requirement(requirement)

    # Setup worker info
    elapsed: int = 0
    workers: List[Optional[Worker]] = [Worker() for _ in range(5)]
    # Loop through tasks to assign
    while tasks:
        # Try to assign all workers
        for worker in workers:
            # If not task assigned
            if not worker.task:
                # Try giving the next possible assignable task
                if worker.assign(next((s for s in tasks.values() if not s.requirements), None)):
                    # If a task has been assigned, delete it from the list
                    del tasks[worker.task.key]

        # Update all workers
        for worker in workers:
            worker.update()

        # Increment counter
        elapsed += 1

    # List to keep track of indices of workers that have completed their job
    deleted: List[int] = []
    while workers:
        # Loop through all workers
        for i, worker in enumerate(workers):
            # Update the worker and add him to the "to delete" list if complete
            if worker.update():
                deleted.append(i)

        # Remove workers that have finished
        while deleted:
            del workers[deleted.pop()]

        # Increment counter
        elapsed += 1

    print("Part two time:", elapsed)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
