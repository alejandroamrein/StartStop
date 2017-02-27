import { NgModule } from '@angular/core';
import { IonicApp, IonicModule } from 'ionic-angular'

import { StartStopApp} from './app.component';

import { AboutComponent } from '../pages/about/about.component';
import { LoginComponent } from '../pages/login/login.component';
import { SetupComponent } from '../pages/setup/setup.component';
import { HomeComponent } from '../pages/home/home.component';
import { StartTimeDialog } from '../pages/home/home.startTime.dialog';
import { HistoryDialog } from '../pages/home/home.history.dialog';

import { DataService } from '../services/data';

@NgModule({
    declarations: [
        StartStopApp,
        AboutComponent,
        LoginComponent,
        SetupComponent,
        HomeComponent,
        StartTimeDialog,
        HistoryDialog
    ],
    imports: [
        IonicModule.forRoot(StartStopApp)
    ],
    bootstrap: [IonicApp],
    entryComponents: [
        StartStopApp,
        AboutComponent,
        LoginComponent,
        SetupComponent,
        HomeComponent,
        StartTimeDialog,
        HistoryDialog
    ],
    providers: [
        DataService
    ]
})
export class AppModule { }
