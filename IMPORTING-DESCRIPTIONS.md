# Importing photo descriptions

This is a guide to exporting the textual descriptions you added to your PhotoStation library from your Synology NAS (Network Attached Storage) and importing them into OSPhoto.

### Outline explanation of the steps involved:

1. On the Synology NAS, export the photo metadata (titles and descriptions) to a CSV file
1. Get that file where OSPhoto can read it
1. Start (or restart) OSPhoto


### Step 1:

Get a root terminal on your Synology NAS server (the one with PhotoStation running on it - it doesn't matter if PhotoStation is running), then run the following commands
``` shell
    $ cd /volume1/public  # or a similar share location that you can browse from another computer)

    $ psql -U postgres -d photo -c "COPY (SELECT * FROM photo_image) TO stdout DELIMITER ',' CSV HEADER" > photo_image.csv

    $ psql -U postgres -d photo -c "COPY (SELECT * FROM photo_share) TO stdout DELIMITER ',' CSV HEADER" > photo_share.csv
```

### Step 2:

Copy the CSV export files from your Synology NAS server to where you are running or intend to run OSPhoto

Put these CSV files in OSPhoto's persistent storage location, in a sub-directory called `import`

For example, if OSPhoto's docker `-v` volume mapping is:
``` shell
    -v '/path/for/os-photos/metadata':'/AppData':'rw'
```

Then copy the CSV files into
``` shell
    /path/for/os-photo/metadata/import/
```

### Step 3:

Start (or restart) OSPhoto.

Open the docker logs and you should see information messages showing the progress of importing the photo descriptions and album cover photo selections.

Note that OSPhoto will try to do the right thing:
- if a description doesn't already exist for the photo, it'll import it
- if the photo file can't be found by OSPhoto, then it'll save the title and description for later matching

When the import completes, the input file is moved to either `./success` if the import file was successfully read, or `./failed` if there was a problem with the file. Again, the docker logs will have full details.
