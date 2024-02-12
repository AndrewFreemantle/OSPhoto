## OSPhoto

OSPhoto is an open source replacement for [Synology's PhotoStation](https://www.synology.com/en-uk/dsm/feature/photo_station), a photo and video storage gallery that you self-host on your [own server](https://en.wikipedia.org/wiki/Network-attached_storage).

### About

This project aims to be a simple, private (and privacy-focussed) photo album that is compatible with Synology's DS photo [mobile](https://play.google.com/store/apps/details?id=com.synology.dsphoto) [apps](https://itunes.apple.com/app/ds-photo/id321493106) and [desktop apps](https://www.synology.com/en-uk/dsm/6.2/software_spec/photo_station#affiliated_utility__photo_station_uploader) for browsing and uploads, with a future aim of a simple web interface too.

### Rationale

There are a few reasons for this project, the main one being that Synology's PhotoStation has been superseded by a [Photos](https://www.synology.com/en-uk/dsm/feature/photos) application, which requires a version of their Disk Station Manager (DSM) which some older Synology hardware doesn't support.

### Progress

- [x] Initial project creation
- [ ] **Started:** Basic DS photo app support (login, browse) - [API ref](https://github.com/jamesbo13/syno-photostation-api)
  - [x] Login
  - [ ] Browse
  - [ ] Login Authentication (secured accounts)
- [ ] Docker installable with a read-only volume of photos
- [ ] Import descriptions, titles and metadata from Synology's PhotoStation
- [ ] Photo uploads and album management (via apps)
- [ ] Tagging & descriptions (importable from Photo Station)
- [ ] Basic web browser interface: folders as albums, navigation, etc
- [ ] Web interface administration (uploads, tagging, user accounts, etc)
- [ ] Plugin support

### Contributing

Any help is very welcome!

- [x] [Raise an issue](https://github.com/AndrewFreemantle/OSPhoto/issues) to request a feature or report a bug
- [x] Fellow developers can pick up any issue (please see `CONTRIBUTING.md` for some guidance)
