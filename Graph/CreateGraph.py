# sphinx_gallery_thumbnail_number = 3
import matplotlib.pyplot as plt
import numpy as np
from matplotlib.ticker import (MultipleLocator, AutoMinorLocator)
import json
import sys


def parse(filename, graphName):
    vector = {}
    vector["x"] = []
    vector["y"] = []
    vector["z"] = []
    vector["value"] = []
    with open(filename) as f:
        graphjson = json.load(f)

        graph = graphjson["frames"]
        for frame in graph:
            dt = frame["dt"]
            if isinstance(frame[graphName], dict):
                x = frame[graphName]["x"]
                y = frame[graphName]["y"]
                z = frame[graphName]["z"]
                vector["x"].append((dt, x))
                vector["y"].append((dt, y))
                vector["z"].append((dt, z))
            else:
                value = frame[graphName]
                vector["value"].append((dt, value))


    return vector


def plot(vector, graph_name):
    fig, ax = plt.subplots()
    #ax.yaxis.set_major_locator(MultipleLocator(0.5))
    for graphName in vector.keys():
        x = list(map(lambda xy: xy[0], vector[graphName]))
        y = list(map(lambda xy: xy[1], vector[graphName]))

        ax.plot(x, y, label=graphName)
    plt.xlabel("Time ")
    plt.ylabel("Value")
    plt.title(graph_name)
    plt.grid(True)
    plt.legend()
    plt.show()


# read and parse the data from json
# plot them to matplotlib
def main():
    filename = sys.argv[1]
    for graph_index in range(2, len(sys.argv)) :
        graph_name = sys.argv[graph_index]
        print(graph_name)
        if graph_name == filename:
            continue
        graph = parse(filename, graph_name)
        plot(graph, graph_name)


if __name__ == '__main__':
    main()
