[![Build Status](https://travis-ci.org/sonicos/AmiiboGen.svg?branch=master)](https://travis-ci.org/sonicos/AmiiboGen) [![Code Climate](https://codeclimate.com/github/sonicos/AmiiboGen/badges/gpa.svg)](https://codeclimate.com/github/sonicos/AmiiboGen) [![Test Coverage](https://codeclimate.com/github/sonicos/AmiiboGen/badges/coverage.svg)](https://codeclimate.com/github/sonicos/AmiiboGen/coverage) [![Issue Count](https://codeclimate.com/github/sonicos/AmiiboGen/badges/issue_count.svg)](https://codeclimate.com/github/sonicos/AmiiboGen)

# AmiiboGen
Tool to generate random unique ids for Amiibo Tags

## Prerequisites

You will need to provide your own key_retail.bin file and put it in the directory with AmiiboGen.exe

## Usage

    AmiiboGen.exe -i <input> -o <output> -c <count> -d -s -p

    Options:
      -i FILE, --input=FILE     Required. Input file or wildcard
      -o FILE, --output=FILE    Output file prefix (or folder for wildcard). Default uses current filename/folder
      -c INT, --count=INT       (Default: 1) Number of generated tags per Amiibo
      -d, --own-directory       Creates a sub-directory for each Amiibo
      -s, --standardize         Standardize output filename based on Amiibo Data
      -p, --set-prefix          Prefix the filename with the Set 'Shortname' (requires -s to have effect)
      --help                    Display the help screen.

## Example

    AmiiboGen.exe -i myAmiibo.bin

Produces myAmiibo_99B2D583303381.bin in the working directory

    AmiiboGen.exe -i Sheik.bin -s -p

Produces [SSB]_Shiek_4DE1A40ADFBB08.bin in the working directory

    AmiiboGen.exe -i ZeldaFile.bin -s -d

Produces Zelda_BotW_937D32795962CC.bin in the subfolder Zelda_BotW\

    AmiiboGen.exe -i *.bin -o generated -s -d -c 50

Produces 50 uniquely ID'd bin files, prefixed and standardized, each in thier own subfolder, all under the subfolder Generated\

### Additional Information

This project uses my [libamiibo.noimage](https://github.com/sonicos/libamiibo.noimage) project, which is forked from [libamiibo](https://github.com/Falco20019/libamiibo). You can use either library. The only difference is that libamiibo.noimage has all the bitmaps removed, reducing the filesize substantially.
