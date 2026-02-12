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
    parser.add_argument("--i", required=True, type=str)
    args = parser.parse_args()
    
    I = args.i
    
    # load the workflow from file, assign it to variable named prompt_workflow
    #prompt_workflow = json.load(open('C:/Projekte/VisualContentGenAR/Python/api/txt2objfast_api.json', 'r', encoding='utf-8'))
    prompt_workflow = json.load(open('D:/Projects/VisualContentGenAR/Python/api/animatePainting_api.json', 'r', encoding='utf-8'))

    set_image = prompt_workflow["17"]

    #set_coords["inputs"]["points_store"] = "{\"positive\":[{\"x\":1260.0,\"y\":600.0}],\"negative\":[{\"x\":0,\"y\":0}]}"
    #set_coords["inputs"]["coordinates"] = "[{\"x\":1260.0,\"y\":600.0}]"
    set_image["inputs"]["image"] = I
   
    queue_prompt(prompt_workflow)