# Importing media comments

This is a guide to exporting the comment threads added to your PhotoStation library from your Synology NAS (Network Attached Storage) and importing them into OSPhoto.

### Outline explanation of the steps involved:

1. On the Synology NAS, export the comments to a CSV file
1. Get that file where OSPhoto can read it
1. Start (or restart) OSPhoto


### Step 1:

Get a root terminal on your Synology NAS server (the one with PhotoStation running on it - it doesn't matter if PhotoStation is running), then run the following commands
``` shell
    $ cd /volume1/public  # or a similar share location that you can browse from another computer)

    $ psql -U postgres -d photo -c "COPY (SELECT c.*, i.path FROM photo_comment c LEFT JOIN photo_image i ON c.photo_id = i.id) TO stdout DELIMITER ',' CSV HEADER" > photo_comment.csv
```

### Step 2:

Copy the CSV export file from your Synology NAS server to where you are running or intend to run OSPhoto

Put these CSV export file in OSPhoto's persistent storage location, in a sub-directory called `import`

For example, if OSPhoto's docker `-v` volume mapping is:
``` shell
    -v '/path/for/os-photos/metadata':'/AppData':'rw'
```

Then copy the CSV export file into
``` shell
    /path/for/os-photo/metadata/import/
```

### Step 3:

Start (or restart) OSPhoto.

Open the docker logs and you should see information messages showing the progress of importing the comments.

Note that OSPhoto will try to do the right thing:
- if the media file can't be found by OSPhoto, then it'll save the comment for later matching

When the import completes, the input file is moved to either `./success` if the import file was successfully read, or `./failed` if there was a problem with the file. Again, the docker logs will have full details.

> Note that while you'll see the imported comments against your photos and videos, you may want to enable commenting in OSPhoto by setting the environment variable 'ALLOW_COMMENTS' to any value. See the main README.md for further details 
