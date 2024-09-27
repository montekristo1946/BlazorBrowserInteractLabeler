
from tqdm import tqdm
import io
import json

from BIL.BILDto import BILDto

def LoadBILDtoInFuile(full_path_json:str)->BILDto:
    print("Load:", full_path_json)
    with io.open(full_path_json, 'r', encoding="utf-8") as f:
        current_cocoDto = json.load(f)
        labels = current_cocoDto.get('Labels')
        images = current_cocoDto.get('Images')
        annotations = current_cocoDto.get('Annotations')

    dataset = BILDto(Labels=labels, Annotations=annotations, Images=images)
    return dataset;