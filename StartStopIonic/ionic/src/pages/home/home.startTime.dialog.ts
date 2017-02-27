import { Component } from '@angular/core';
import { ModalController, ViewController, NavParams } from 'ionic-angular'; 

@Component({
    templateUrl: 'home.startTime.dialog.html'
})
export class StartTimeDialog {

    public datum: Date = new Date();

    constructor(private modal: ModalController,
        public view: ViewController,
        public navParams: NavParams) {
        this.datum = this.navParams.get('datum');
        console.log(this.datum);
    }

    save() {
        this.view.dismiss({ datum: this.datum });
    }

    dismiss() {
        this.view.dismiss(null);
    }
}
