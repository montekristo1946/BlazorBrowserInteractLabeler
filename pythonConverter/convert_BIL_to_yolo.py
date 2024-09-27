import os
from tqdm import tqdm
import numpy as np
import cv2
import glob
import datetime

from Common import LoadBILDtoInFuile

WIDTH_IMAGE = 640
HEIGHT_IMAGE = 640

BIL_TO_YOLO = {
    1: 0,
    2: 1,
    3: 2,
    4: 3,
    5: 4,
}


def _load_img(path_images, color=0):
    img_src = cv2.imread(path_images, color)
    if img_src is None:
        raise Exception(f"[load_image] {path_images}")
    img_src = cv2.resize(img_src, (WIDTH_IMAGE, HEIGHT_IMAGE))
    return img_src

def _get_path_from_short_root_path(all_path, image_file):
    index_images = all_path[:, 0]
    path_img_complex = all_path[np.where(index_images == image_file)]
    if path_img_complex.shape[0] != 1:
        raise Exception(f"[_get_path_from_np] img is None; {image_file}")

    path_image = path_img_complex[0][1]
    return path_image


def _create_np_path(input_folder, ext):
    all_path = [path for path in glob.iglob(input_folder + f'**/*.{ext}', recursive=True)]
    all_path.sort()

    count_annotations = len(all_path)
    all_annot_path_np = np.zeros((count_annotations, 2), dtype=object)

    for i in range(count_annotations):
        path = all_path[i]
        current_name = os.path.basename(path)
        name_annot, _ = os.path.splitext(current_name)
        all_annot_path_np[i, 0] = name_annot
        all_annot_path_np[i, 1] = path

    return all_annot_path_np


def _convert_label(concurrent_annot):
    rect_label = []

    for annot in concurrent_annot:
        label_id = annot.LabelId
        label_Pattern = annot.LabelPattern
        points = annot.Points
        id_yolo_rect = BIL_TO_YOLO[label_id]

        if label_Pattern != 'Box':
            print(f"[_convert_label] fail label_Pattern:{label_Pattern} annot.Id:{annot.Id}")

        if len(points) != 2:
            raise Exception(f"[_convert_label] point=={len(points)} annot.Id:{annot.Id}")

        x1 = sorted(points, key=lambda item: item.X, reverse=False)[0].X
        y1 = sorted(points, key=lambda item: item.Y, reverse=False)[0].Y

        x2 = sorted(points, key=lambda item: item.X, reverse=True)[0].X
        y2 = sorted(points, key=lambda item: item.Y, reverse=True)[0].Y

        w = x2 - x1
        h = y2 - y1
        xc = x1 + w / 2
        yc = y1 + h / 2
        rect_label.append((id_yolo_rect, [xc, yc, w, h]))
    return rect_label


def _normalized_to_float(x):
    x = _сheck_value(x)
    return float(round(x, 5))


def _сheck_value(x: float) -> float:
    if x < 0:
        return 0
    if x > 1:
        return 1
    return x


def _create_suffix_name(prefix=""):
    ret_line = datetime.datetime.now().strftime("%Y-%m-%d_%H-%M-%S-%f")
    prefix += ret_line
    return prefix


def _create_rect_to_file(boxes):
    if len(boxes) == 0:
        return ""

    ret_str = ''
    for bbox in boxes:
        category_id = int(bbox[0])
        rect_src = bbox[1]
        x = _normalized_to_float(rect_src[0])
        y = _normalized_to_float(rect_src[1])
        w = _normalized_to_float(rect_src[2])
        h = _normalized_to_float(rect_src[3])
        ret_str += '{} {:.6f} {:.6f} {:.6f} {:.6f}\n'.format(category_id, x, y, w, h)

    return ret_str


def _save_images(images_info, dataset, all_path_image, path_output):
    image_file = images_info['NameImages']
    image_file, _ = os.path.splitext(image_file)
    path_images = _get_path_from_short_root_path(all_path_image, image_file)

    img_save = _load_img(path_images, 1)
    id_images = images_info['Id']
    annots_all = dataset.Annotations
    concurrent_annot = [annot for annot in annots_all if annot.ImageFrameId == id_images]
    rect_label = _convert_label(concurrent_annot)
    lines_to_label_file = _create_rect_to_file(rect_label)
    new_name = _create_suffix_name(f"{image_file}_")

    name_label_file = os.path.join(path_output, "Labels", f"{new_name}.txt")
    name_images_file = os.path.join(path_output, "Images", f"{new_name}.jpg")
    with open(name_label_file, 'w') as f:
        f.writelines(lines_to_label_file)

    cv2.imwrite(name_images_file, img_save)
    pass


def _main_loop(all_path_bill, all_path_image, all_path_fon):
    for id_img, (_, path_bill) in enumerate(all_path_bill):
        print(f"Run {path_bill}")
        dataset = LoadBILDtoInFuile(path_bill)
        for images_info in tqdm(dataset.Images):
            _save_images(images_info, dataset, all_path_image, all_path_fon)
        pass


if __name__ == '__main__':
    path_dataset_input = "../Bil_json/"
    path_images = '../images_src'
    path_output = "../train_to_yolo/"

    all_path_bill = _create_np_path(path_dataset_input, 'json')
    all_path_image = _create_np_path(path_images, 'jpg')

    os.makedirs(os.path.join(path_output, "Labels"), exist_ok=True)
    os.makedirs(os.path.join(path_output, "Images"), exist_ok=True)

    _main_loop(all_path_bill, all_path_image, path_output)
