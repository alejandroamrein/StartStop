import { Component } from '@angular/core';
import { FormBuilder, /*FormGroup,*/ Validators } from '@angular/forms';
import { NavController, ToastController } from 'ionic-angular';

import {SetupComponent} from '../setup/setup.component';
import {HomeComponent} from '../home/home.component';

import {DataService} from '../../services/data';

import {User} from '../../model/user';

@Component({
    templateUrl: 'login.component.html'
})
export class LoginComponent {

    public loginForm: any;
    public name;
    public password;

    constructor(private nav: NavController,
        private _form: FormBuilder,
        private dataService: DataService,
        public toastCtrl: ToastController) {
        this.loginForm = this._form.group({
            'name': ["", Validators.required],
            'password': ["", Validators.required]
        });
    }

    presentToast(message: string) {
        let toast = this.toastCtrl.create({
            message: message,
            duration: 3000
        });
        toast.present();
    }

    gotoSetup() {
        this.nav.setRoot(SetupComponent);
    }

    submit() {
        this.dataService.login(this.name, this.password).subscribe(
            data => {
                var response = data.text();
                if (response == "true") {
                    let user: User = new User(1, this.name, "email");
                    this.dataService.setCurrentUser(user);
                    this.nav.setRoot(HomeComponent);
                }
                else {
                    this.presentToast(`Benutzer oder Passwort falsch`);
                }
            },
            err => console.error(err),
            () => {
                console.log('login completed');
            }
        );
    }
}