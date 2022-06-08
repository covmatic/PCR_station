# PCR API for Biorad CFX Maestro

## v1.7.6 2022-06-08
### Fixed
- Increased FAM, ROX and Cy5 thresholds to 5000 to avoid false positive results

## v1.7.5 2021-12-16
### Fixed
- Increased FAM, ROX and Cy5 thresholds to 1500

## v1.7.4 2021-09-07
### Fixed
- Application looks for csv results instead of waiting for one minute

## v1.7.3 2021-09-07
### Fixed
- timeout set to 30s trying to solve RunProtocol start problem

## v1.7.2 2021-09-07
### Added
- some logs are printed also by Data Processor Application

## v1.7.1 2021-08-20
### Added
- logs are saved to folder with a date-time filename;
- print version in the main window

### Fixed
- loggers use try-catch.

## v1.6.2 2021-08-19
### Fixed
- RunProtocol is enqueued and executed in timer function.

## v1.6.1 2021-08-18
### Fixed
- Increased wait time after run to accomplish API specification

## v1.6 2021-02-10
### Fixed
- CSV results are now correctly converted with dot as decimal separator.