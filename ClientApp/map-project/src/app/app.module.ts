import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AgmCoreModule, MarkerManager, GoogleMapsAPIWrapper } from '@agm/core';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyCWWwyU7qmp7WxzhvR07KhA0K4a7IEr0S0'
    }),
    BrowserModule,
    HttpClientModule
  ],
  providers: [MarkerManager, GoogleMapsAPIWrapper],
  bootstrap: [AppComponent]
})
export class AppModule { }
