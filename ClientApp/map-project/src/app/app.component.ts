import { Component, OnInit, AfterViewInit, AfterContentInit } from '@angular/core';
import { MouseEvent } from '@agm/core'
import { MarkerManager, AgmMarker } from '@agm/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})


export class AppComponent implements  AfterContentInit {
  title = 'map-project';

  latitude: number =51.9189046;
  longitude: number =19.1343786;
  mapType = 'hybrid';
  public clickedPoint: AgmMarker = this.createMarker(this.longitude, this.latitude);

  constructor(private httpClient: HttpClient) {
  }
  ngAfterContentInit(): void {
    this.getLocation();
  }

  
  public onRightClickHandle(event: MouseEvent ) {
    const assign = 'anything';
  
  }

  public onClickHandler(event: MouseEvent) {
    this. clickedPoint = this.createMarker(event.coords.lng, event.coords.lat)
  }
  public getLocation(): void {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition((position)=>{
        this.setMapLocation(position.coords.longitude, position.coords.latitude);
      });
  } else {
     console.log("No support for geolocation");
    }
  }

  private setMapLocation(longitude: number, latitude: number)
  {
    this.clickedPoint = this.createMarker(longitude, latitude)
    this.latitude = latitude;
    this.longitude = longitude;
  }

  private createMarker(longitude: number, latitude: number){
    return <AgmMarker> {
      longitude: longitude,
      latitude: latitude,
      visible: true,
      title: "This is my place"
    }
  }
}