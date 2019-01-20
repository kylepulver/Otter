#!/bin/sh
install_name_tool -id libcsfml-graphics.2.0.dylib -change @executable_path/../Frameworks/libsfml-system.2.dylib @loader_path/libsfml-system.2.0.dylib libcsfml-graphics.2.0.dylib 
install_name_tool -id libcsfml-graphics.2.0.dylib -change @executable_path/../Frameworks/libsfml-graphics.2.dylib @loader_path/libsfml-graphics.2.0.dylib libcsfml-graphics.2.0.dylib 
install_name_tool -id libcsfml-graphics.2.0.dylib -change @executable_path/../Frameworks/libsfml-window.2.dylib @loader_path/libsfml-window.2.0.dylib libcsfml-graphics.2.0.dylib 

install_name_tool -id libcsfml-window.2.0.dylib -change @executable_path/../Frameworks/libsfml-window.2.dylib @loader_path/libsfml-window.2.0.dylib libcsfml-window.2.0.dylib 
install_name_tool -id libcsfml-window.2.0.dylib -change @executable_path/../Frameworks/libsfml-system.2.dylib @loader_path/libsfml-system.2.0.dylib libcsfml-window.2.0.dylib

install_name_tool -id libcsfml-audio.2.0.dylib -change @executable_path/../Frameworks/libsfml-audio.2.dylib @loader_path/libsfml-audio.2.0.dylib libcsfml-audio.2.0.dylib 

install_name_tool -id libsfml-graphics.2.0.dylib -change @executable_path/../Frameworks/libsfml-window.2.dylib @loader_path/libsfml-window.2.0.dylib libsfml-graphics.2.0.dylib
install_name_tool -id libsfml-graphics.2.0.dylib -change @executable_path/../Frameworks/libsfml-system.2.dylib @loader_path/libsfml-system.2.0.dylib libsfml-graphics.2.0.dylib  
install_name_tool -id libsfml-graphics.2.0.dylib -change @executable_path/../Frameworks/freetype.framework/Versions/A/freetype @loader_path/libfreetype.dylib libsfml-graphics.2.0.dylib  

install_name_tool -id libsfml-window.2.0.dylib -change @executable_path/../Frameworks/libsfml-system.2.dylib @loader_path/libsfml-system.2.0.dylib libsfml-window.2.0.dylib
