import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { Platform, Nav, MenuController } from 'ionic-angular';
import { StatusBar } from 'ionic-native';

import { AboutComponent } from '../pages/about/about.component';
import { LoginComponent } from '../pages/login/login.component';
import { SetupComponent } from '../pages/setup/setup.component';
import { HomeComponent } from '../pages/home/home.component';

import { DataService } from '../services/data';

@Component({
    templateUrl: 'app.component.html'
})
export class StartStopApp implements AfterViewInit {
    @ViewChild(Nav) nav: Nav;

    ngAfterViewInit(): void {
        //console.log('ngAfterViewInit');
    }

    rootPage: any = HomeComponent;

    constructor(platform: Platform,
        private dataService: DataService,
        private menu: MenuController) {

        console.log('StartStopApp::ctor');

        // rootPage: HomePage;
        let temp = localStorage.getItem('url');
        if (temp == null || temp == '') {
            this.rootPage = SetupComponent;
        }
        else {
            temp = localStorage.getItem('user');
            if (temp == null || temp == '') {
                this.rootPage = LoginComponent;
            } else {
                this.rootPage = HomeComponent;
            }
        }

        platform.ready().then(() => {
            // Okay, so the platform is ready and our plugins are available.
            // Here you can do any higher level native things you might need.
            StatusBar.styleDefault();
        });
    }

    gotoAbout() {
        console.log('gotoAbout');
        this.nav.push(AboutComponent);
        this.menu.close();
    }

    gotoSetup() {
        console.log('gotoSetup');
        this.nav.setRoot(SetupComponent);
        this.menu.close();
    }

    logout() {
        this.dataService.logout();
        this.nav.setRoot(LoginComponent);
        this.menu.close();
    }
}
