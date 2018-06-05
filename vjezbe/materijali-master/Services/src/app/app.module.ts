import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule, JsonpModule } from '@angular/http';

import { SimpleData } from "./services/simple-data.service"

import { AppComponent } from './app.component';
import { SimpleComponent } from './simple/simple.component';
import { Simple2Component } from './simple2/simple2.component';
import { ProductComponent } from './product/product.component';
import { ClockComponent } from './clock/clock.component';

import { SignalRService } from './services/signalR.service'

@NgModule({
  declarations: [
    AppComponent,
    SimpleComponent,
    Simple2Component,
    ProductComponent,
    ClockComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    JsonpModule
  ],
  providers: [SimpleData, SignalRService],
  bootstrap: [AppComponent]
})
export class AppModule { }
