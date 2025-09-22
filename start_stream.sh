#!/bin/bash

# Überprüfen, ob die Datei 'warning-text.txt' existiert
if [[ ! -f warning-text.txt ]]; then
  echo "Fehler: Die Datei 'warning-text.txt' wurde nicht gefunden!"
  exit 1
fi

# Führe den ffmpeg Befehl aus
ffmpeg -f v4l2 \
       -i /dev/video0 \
       -vf "drawtext=textfile='warning-text.txt':x=(w-text_w)/2:y=(h-text_h)/2:fontsize=100:fontcolor=white:box=1:boxcolor=black@0.5:reload=1" \
       -pix_fmt yuv420p \
       -framerate 60 \
       -s 1920x1080 \
       -f rawvideo - | ffplay -fs -f rawvideo -video_size 1920x1080 -pixel_format yuv420p -

