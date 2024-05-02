## OSPhoto

<div style="display: flex; align-items: center; gap: 2em; width: 85%; margin-left: auto; margin-right: auto;">
  <div>
    <img src="./Docs/OSPhoto-logo-no-background.png" alt="OSPhoto's logo - a flat, 2-dimensional design depicting a large sun in the top left over 2 mountain peaks with a rounded-square border">
  </div>
  <div style="display: flex; flex-direction: column; justify-content: center">
    <p>OSPhoto is an open source replacement for <a href="https://www.synology.com/en-uk/dsm/feature/photo_station">Synology's PhotoStation</a>, a photo and video storage gallery that you self-host on your <a href="https://en.wikipedia.org/wiki/Network-attached_storage">own server</a>.</p>
  </div>
</div>

### About

This project aims to be a simple, private (and privacy-focussed) photo album that is compatible with Synology's DS photo [mobile](https://play.google.com/store/apps/details?id=com.synology.dsphoto) [apps](https://itunes.apple.com/app/ds-photo/id321493106) and [desktop apps](https://www.synology.com/en-uk/dsm/6.2/software_spec/photo_station#affiliated_utility__photo_station_uploader) for browsing and uploads, with a future aim of a simple web interface too.

It works as a simple interface (or API) over your existing curated folders of photos and videos.

### Installation

Installation is a [Docker container](https://hub.docker.com/r/andrewfreemantle/os-photo) running on a Network Attached Storage server:

```shell
docker run \
  -d \
  --name='os-photo' \
  --net='bridge' \
  -e 'USERS'='SomeUser=TheirPassword;SomeOtherUser=AndTheirPassword' \
  -p '5000:5000/tcp' \
  -v '/path/to/a/photo-collection/':'/Media':'rw' \
  -v '/path/to/some/persistent/location/for/os-photos/metadata':'/AppData':'rw' \
  'andrewfreemantle/os-photo'
```

Once running, OSPhoto will accept http connections on port `5000` using any of the credentials specified in `"USERS"`.

### Rationale

There are a few reasons for this project, the main one being that Synology's PhotoStation has been superseded by a [Photos](https://www.synology.com/en-uk/dsm/feature/photos) application, which requires a version of their Disk Station Manager (DSM) which some older Synology hardware doesn't support.

### Progress

- [x] Initial project creation
- [x] Basic DS photo app support (login, browse) - [API ref](https://github.com/jamesbo13/syno-photostation-api)
- [x] Docker installable with a read-only volume of photos
- [x] Import descriptions, titles and metadata from Synology's PhotoStation
- [ ] [HEIC/HEIF](https://en.wikipedia.org/wiki/High_Efficiency_Image_File_Format) photo file support
- [x] Photo uploads, move and delete (via apps)
- [ ] Album management (via apps)
- [ ] Tags and comments (importable from Photo Station)
- [ ] Basic web browser interface: folders as albums, navigation, etc
- [ ] Web interface administration (uploads, tagging, user accounts, etc)
- [ ] Plugin support

### Contributing

Any help is very welcome!

- [x] [Raise an issue](https://github.com/AndrewFreemantle/OSPhoto/issues) to request a feature or report a bug
- [x] Fellow developers can pick up any issue (please see `CONTRIBUTING.md` for some guidance)
