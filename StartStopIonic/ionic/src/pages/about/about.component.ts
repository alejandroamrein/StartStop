import { Component } from '@angular/core';
import { NavController } from 'ionic-angular';
import { MyLocation, Geolocation, GoogleMapsEvent, GoogleMapsLatLng, GoogleMap, CameraPosition, GoogleMapsMarkerOptions, GoogleMapsMarker } from 'ionic-native';

@Component({
    templateUrl: 'about.component.html'
})
export class AboutComponent {
    map: any;

    constructor(public navCtrl: NavController) {
    }

    ngAfterViewInit() {
        //this.loadMap([{ 'longitud': 43.0741904, 'latitude': -89.3809802 }]);
        //this.getPosition();
        this.loadMap2();
    }

    loadMap2() {
        // make sure to create following structure in your view.html file
        // <ion-content>
        //  <div #map id="map"></div>
        // </ion-content>
        //let longitud = coords[0]['longitude'];
        //let latitude = coords[0]['latitude'];

        // create a new map by passing HTMLElement
        let element: HTMLElement = document.getElementById('map');

        let ionic: GoogleMapsLatLng = new GoogleMapsLatLng(-34.6209212, -58.4458847);

        let map = new GoogleMap(element);

        // listen to MAP_READY event
        map.one(GoogleMapsEvent.MAP_READY).then(() => console.log('Map is ready!'));

        // create LatLng object
        //let ionic: GoogleMapsLatLng = new GoogleMapsLatLng(-34.6209212, -58.4458847);
        //let ionic: GoogleMapsLatLng = new GoogleMapsLatLng(longitud, latitude);

        //map.getMyLocation().then((myLoc: MyLocation) => {
            // create CameraPosition
            let position: CameraPosition = {
                target: ionic,
                zoom: 15,
                tilt: 30
            };

            // move the map's camera to position
            //map.moveCamera(position);

            // move the map's camera to position
            map.setCenter(ionic);
            map.setZoom(15);

            // create new marker
            let markerOptions: GoogleMapsMarkerOptions = {
                position: ionic,
                title: 'Ionic'
            };

            map.addMarker(markerOptions)
                .then((marker: GoogleMapsMarker) => {
                    marker.showInfoWindow();
                });
        //});

    }

    //ionViewLoaded() {
    //    this.getPosition();
    //}

    getPosition(): any {
        Geolocation.getCurrentPosition().then(res => {
            //alert('getPosition: ' + res.coords);
            console.log(res.coords);
            let coords = [{
                'longitude': res.coords.longitude,
                'latitude': res.coords.latitude
            }];
            console.log(coords);
            //alert('getPosition: ' + coords);
            this.loadMap(coords);
        });
    }

    loadMap(coords: any[]) {
        //alert('loadMap: ' + coords);
        console.log(coords);
        let longitud = coords[0]['longitude'];
        let latitude = coords[0]['latitude'];
        // let location: crea un objeto con las coordenadas latitude y longitud y es pasada a las // opciones de google maps.

        let location = new GoogleMapsLatLng(latitude, longitud);
        //alert('location ' + location);
        this.map = new GoogleMap('map', {
            'backgroundColor': 'white',
            'controls': {
                'compass': true,
                'myLocationButton': true,
                'indoorPicker': true,
                'zoom': true,
            },
            'gestures': {
                'scroll': true,
                'tilt': true,
                'rotate': true,
                'zoom': true
            },
            'camera': {
                'latLng': location,
                'tilt': 30,
                'zoom': 15,
                'bearing': 50
            }
        });
        //alert('Event ' + GoogleMapsEvent.MAP_READY);
        this.map.on(GoogleMapsEvent.MAP_READY).subscribe(() => {
            alert('Map is ready!');
            console.log('Map is ready!');
        });
    }
}