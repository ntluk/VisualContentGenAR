import json
from urllib import request, parse
import random


def queue_prompt(prompt_workflow):
    p = {"prompt": prompt_workflow}
    data = json.dumps(p).encode('utf-8')
    req = request.Request("http://127.0.0.1:8188/prompt", data=data)
    request.urlopen(req)    
    
# load the workflow from fil, assign it to variable named prompt_workflow
prompt_workflow = json.load(open('AAEPlus_Scribble_api.json', 'r', encoding='utf-8'))
# prompt_workflow = json.load(open('lindenbergSCribbleworkflow_api.json', 'r', encoding='utf-8'))
# prompt_workflow = json.load(open('HOME_AAEPlus_Scribble_api.json', 'r', encoding='utf-8'))
load_canvasimg = prompt_workflow["215"]
load_canvasimg["inputs"]["image"] = "C:\\Users\\TinderAR\\Desktop\\AiArtExtendedBuild\\FinalNormal\\Assets\\SavedTextures\\_CanvasTexture.png"
queue_prompt(prompt_workflow)