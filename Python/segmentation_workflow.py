import json
from urllib import request, parse
import random
import argparse


def queue_prompt(prompt_workflow):
    p = {"prompt": prompt_workflow}
    data = json.dumps(p).encode('utf-8')
    req =  request.Request("http://127.0.0.1:8188/prompt", data=data)
    request.urlopen(req)    
  
if __name__ == "__main__":  
    parser = argparse.ArgumentParser()
    parser.add_argument("--x", required=True, type=str)
    parser.add_argument("--y", required=True, type=str)
    args = parser.parse_args()
    
    X = args.x
    Y = args.y
    
    # load the workflow from file, assign it to variable named prompt_workflow
    prompt_workflow = json.load(open('C:/Projekte/NiklasTluk/VisualContentGenAR/Python/Gen3D_api.json', 'r', encoding='utf-8'))

    set_coords = prompt_workflow["115"]


    #set_coords["inputs"]["points_store"] = "{\"positive\":[{\"x\":1260.0,\"y\":600.0}],\"negative\":[{\"x\":0,\"y\":0}]}"
    #set_coords["inputs"]["coordinates"] = "[{\"x\":1260.0,\"y\":600.0}]"
    set_coords["inputs"]["points_store"] = "{\"positive\":[{\"x\":" + X + ",\"y\":" + Y + "}],\"negative\":[{\"x\":0,\"y\":0}]}"
    set_coords["inputs"]["coordinates"] = "[{\"x\":" + X + ",\"y\":" + Y + "}]"



    queue_prompt(prompt_workflow)