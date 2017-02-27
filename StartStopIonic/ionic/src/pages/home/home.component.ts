import { Component, ViewChild } from '@angular/core';
import { Content, NavController, ModalController } from 'ionic-angular'; 
import { Loading, LoadingController, ToastController } from 'ionic-angular';

import { StartTimeDialog } from './home.startTime.dialog';
import { HistoryDialog } from './home.history.dialog';
         
import { DataService } from '../../services/data';
         
import { User } from '../../model/user';
import { SelectItem } from '../../model/selectItem';

@Component({
    templateUrl: 'home.component.html'
})
export class HomeComponent {
    @ViewChild(Content) content: Content;
    public username: any = 'Not logged';
    private user: User = null;
    public foundBooks;
    public searchText;
    //public loading = false;
    public errorMessage: string = null;
    public infoMessage: string = null;
    public infoMessage1: string = null;
    public infoMessage2: string = null;
    public infoMessage3: string = null;
    public op;
    public text: string = null;
    public state = 'stopped'; // 'started', 'paused', 'stopped'
    public projekt: string = null;
    private lastProjekt: string = null;
    public tarifKategorie: string = null;
    private lastTarifKategorie: string = null;
    public lohnKategorie: string = null;
    private lastLohnKategorie: string = null;
    public startTime = '10:57:00';
    public ellapsed = '20:09';
    public projekte: SelectItem[] = [];
    public lohnKategorien: SelectItem[] = [];
    public tarifKategorien: SelectItem[] = [];

    constructor(private nav: NavController,
        private dataService: DataService,
        private loadingCtrl: LoadingController,
        public modal: ModalController,
        public toastCtrl: ToastController) {
        console.log('HomeComponent::ctor');
        this.refreshLoginInfo();
        console.log('HomeComponent::ctor::initState');
        this.initState();
        console.log('HomeComponent::ctor::end');
    }

    presentToast(message: string, duration: number) {
        let toast = this.toastCtrl.create({
            message: message,
            duration: duration
        });
        toast.present();
    }

    initState() {
        let loader = this.presentLoader();
        this.dataService.getState().subscribe(
            data => {
                var state = data.json();
                // this.infoMessage = `Aktueller Zustand: ${state.State}`;
                this.presentToast(`Aktueller Zustand: ${state.State}`, 3000);
                console.log('State: ' + state);
                //if (state != null) {
                this.state = state.State;
                this.lastProjekt = state.Projekt;
                this.lastLohnKategorie = state.LohnKategorie;
                this.lastTarifKategorie = state.TarifKategorie;
                this.text = state.Text;
                this.ellapsed = state.TimeIntervall;
                this.loadProjekte();
                this.loadTarifkategorien();
                //}
                console.log('state: ' + state.State);
            },
            err => {
                this.errorMessage = "Fehler beim Laden aktuellen Zustand";
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('getState completed');
            }
        );
    }

    loadTarifkategorien() {
        let loader = this.presentLoader();
        this.dataService.getTarifkategorien().subscribe(
            data => {
                this.tarifKategorien = data.json();
                this.presentToast(`${this.tarifKategorien.length} TarifKategorien geladen`, 3000);
                //this.infoMessage3 = `${this.tarifKategorien.length} TarifKategorien geladen`;
                if (this.lastTarifKategorie != null) {
                    this.tarifKategorie = this.lastTarifKategorie;
                } else {
                    this.tarifKategorie = this.tarifKategorien.length > 0 ? this.tarifKategorien[0].Value : null;
                }
                console.log('tarifkategorien: ' + this.tarifKategorien.length);
            },
            err => {
                this.errorMessage = "Fehler beim Laden Tarifkategorien";
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('getTarifkategorien completed');
            }
        );
    }

    refresh() {
        this.initState();
    }

    loadProjekte() {
        //this.infoMessage = `Stammdaten werden geladen...`;
        let loader = this.presentLoader();
        this.presentToast('Stammdaten werden geladen...', 3000);
        this.dataService.getProjekte().subscribe(
            data => {
                this.projekte = data.json();
                //this.infoMessage1 = `${this.projekte.length} Projekte geladen`;
                this.presentToast(`${this.projekte.length} Projekte geladen`, 3000);
                // TODO: last projekt setzen
                if (this.lastProjekt != null) {
                    this.projekt = this.lastProjekt;
                } else {
                    this.projekt = this.projekte.length > 0 ? this.projekte[0].Value : null;
                }
                console.log('projekte: ' + this.projekte.length);
                this.reloadLohnkategorien(true);
            },
            err => {
                this.errorMessage = "Fehler beim Laden Projekte";
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('getProjekts completed');
            }
        );
    }

    reloadLohnkategorien(firstTime) {
        let loader = this.presentLoader();
        this.dataService.getLohnkategorien(this.projekt).subscribe(
            data => {
                this.lohnKategorien = data.json();
                //this.infoMessage2 = `${this.lohnKategorien.length} Lohnkategorien geladen`;
                this.presentToast(`${this.lohnKategorien.length} Lohnkategorien geladen`, 3000);
                if (firstTime) {
                    this.lohnKategorie = this.lastLohnKategorie;
                } else {
                    this.lohnKategorie = this.lohnKategorien.length > 0 ? this.lohnKategorien[0].Value : null;
                }
                console.log('lohnkategorien: ' + this.lohnKategorien.length);
                this.clearMessagesDelay(5000);
            },
            err => {
                this.errorMessage = "Fehler beim Laden Lohnkategorien";
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('getLohnkategorien completed');
            }
        );    
    }

    refreshLoginInfo() {
        let temp = localStorage.getItem('user');
        if (temp == null || temp == 'null') {
            this.username = 'Not logged';
            this.user = null;
        } else {
            this.user = JSON.parse(temp);
            this.username = this.user.name;
        }
    }

    loader: Loading;

    presentLoader(): Loading {
        let loader = this.loadingCtrl.create({
            content: 'Bitte warten',
            duration: 5000
        });
        loader.present();
        return loader;
    }

    clearMessages() {
        this.infoMessage = null;
        this.infoMessage1 = null;
        this.infoMessage2 = null;
        this.infoMessage3 = null;
        this.errorMessage = null;
    }

    clearMessagesDelay(ms: number) {
        var caller = this;
        setTimeout(() => { caller.clearMessages(); }, ms);
    }

    setInfo(i: number, text: string) {
        switch (i) {
            case 0:
                this.infoMessage = text;
                break;
            case 1:
                this.infoMessage1 = text;
                break;
            case 2:
                this.infoMessage2 = text;
                break;
            case 3:
                this.infoMessage3 = text;
                break;
        }
        this.clearMessagesDelay(5000);
    }

    history() {
        let loader = this.presentLoader();
        this.dataService.getHistory().subscribe(
            data => {
                var resp = data.json();
                let modal = this.modal.create(HistoryDialog, { items: resp });
                modal.present();
            },
            err => {
                this.errorMessage = "Fehler beim Laden letzte Eintraege";
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('history() completed');
            }
        );
    }

    cancel() {
        this.clearMessages();
        console.log('cancel() called');
        let loader = this.presentLoader();
        this.dataService.cancel().subscribe(
            data => {
                var resp = data.json();
                if (resp.Success) {
                    this.presentToast(`Erfassung storniert`, 20000);
                    this.state = "stopped";
                    this.ellapsed = resp.TimeIntervall;
                }
                else {
                    this.errorMessage = 'Stop Fehler: ' + resp.Error;
                    alert('Response not ok: ' + resp.Error);
                }
            },
            err => {
                this.errorMessage = "Fehler beim Stoppen";
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('start() completed');
            }
        );
    }

    startEx() {
        let profileModal = this.modal.create(StartTimeDialog, { datum: (new Date()).toISOString() });
        profileModal.onDidDismiss(data => {
            if (data != null) {
                console.log('MODAL DATA: ', data);
                this.clearMessages();
                console.log('startEx() called');
                let loader = this.presentLoader();
                this.dataService.startEx(this.projekt, this.tarifKategorie, this.lohnKategorie, this.text, data.datum).subscribe(
                    data => {
                        var resp = data.json();
                        if (resp.Success) {
                            //this.infoMessage = `Gestartet`;
                            this.presentToast('Erfassung gestartet', 3000);
                            //this.setInfo(0, 'Gestartet');
                            this.state = "started";
                            this.ellapsed = resp.TimeIntervall;
                        }
                        else {
                            this.errorMessage = 'Start Fehler: ' + resp.Error;
                            alert('Response not ok: ' + resp.Error);
                        }
                    },
                    err => {
                        this.errorMessage = `Fehler beim Starten (url: ${this.dataService.getStartUrl()})`;
                        console.error(err);
                    },
                    () => {
                        loader.dismiss();
                        console.log('start() completed');
                    }
                );
            } else {
                console.log('MODAL CANCELLED');
            }
        });
        profileModal.present();
    }

    stopEx() {
        let profileModal = this.modal.create(StartTimeDialog, { datum: (new Date()).toISOString() });
        profileModal.onDidDismiss(data => {
            if (data != null) {
                console.log('MODAL DATA: ', data);
                this.clearMessages();
                console.log('stop() called');
                let loader = this.presentLoader();
                this.dataService.stopEx(data.datum).subscribe(
                    data => {
                        var resp = data.json();
                        if (resp.Success) {
                            //this.infoMessage = `Gestoppt`;
                            //this.setInfo(0, 'Gestoppt');
                            this.presentToast(`Erfassung beendet (${resp.TimeIntervall})`, 20000);
                            this.state = "stopped";
                            this.ellapsed = resp.TimeIntervall;
                        }
                        else {
                            this.errorMessage = 'Stop Fehler: ' + resp.Error;
                            alert('Response not ok: ' + resp.Error);
                        }
                    },
                    err => {
                        this.errorMessage = "Fehler beim Stoppen";
                        console.error(err);
                    },
                    () => {
                        loader.dismiss();
                        console.log('start() completed');
                    }
                );
            } else {
                console.log('MODAL CANCELLED');
            }
        });
        profileModal.present();
    }

    start() {
        this.clearMessages();
        console.log('start() called');
        let loader = this.presentLoader();
        this.dataService.start(this.projekt, this.tarifKategorie, this.lohnKategorie, this.text).subscribe(
            data => {
                var resp = data.json();
                if (resp.Success) {
                    //this.infoMessage = `Gestartet`;
                    this.presentToast(`Erfassung gestartet`, 3000);
                    //this.setInfo(0, 'Gestartet');
                    this.state = "started";
                    this.ellapsed = resp.TimeIntervall;
                }
                else {
                    this.errorMessage = 'Start Fehler: ' + resp.Error;
                    alert('Response not ok: ' + resp.Error);
                }
            },
            err => {
                this.errorMessage = `Fehler beim Starten (url: ${this.dataService.getStartUrl()})`;
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('start() completed');
            }
        );
        // api/StartStop/State/Fritz     
        //    { Action="start", Projekt="", LohnKategorie="", TarifKategorie="", Text="" }
        //    { Action="stop" }
        //    { Action="pause" }
        //    { Action="resume" }
    }

    projektChanged() {
        //this.infoMessage = 'Projekt changed';
        //this.presentToast('Projekt ausgewählt');
        this.reloadLohnkategorien(false);
    }

    stop() {
        this.clearMessages();
        console.log('stop() called');
        let loader = this.presentLoader();
        this.dataService.stop().subscribe(
            data => {
                var resp = data.json();
                if (resp.Success) {
                    //this.infoMessage = `Gestoppt`;
                    //this.setInfo(0, 'Gestoppt');
                    this.presentToast(`Erfassung beendet (${resp.TimeIntervall})`, 20000);
                    this.state = "stopped";
                    this.ellapsed = resp.TimeIntervall;
                }
                else {
                    this.errorMessage = 'Stop Fehler: ' + resp.Error;
                    alert('Response not ok: ' + resp.Error);
                }
            },
            err => {
                this.errorMessage = "Fehler beim Stoppen";
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('start() completed');
            }
        );
    }

    pause() {
        this.clearMessages();
        console.log('pause() called');
        let loader = this.presentLoader();
        this.dataService.pause().subscribe(
            data => {
                var resp = data.json();
                if (resp.Success) {
                    //this.infoMessage = `Gepaused`;
                    //this.setInfo(0, 'Gepaused');
                    this.presentToast(`Erfassung angehalten`, 3000);
                    this.state = "paused";
                    this.ellapsed = resp.TimeIntervall;
                }
                else {
                    this.errorMessage = 'Pause Fehler: ' + resp.Error;
                    alert('Response not ok: ' + resp.Error);
                }
            },
            err => {
                this.errorMessage = "Fehler beim Pausen";
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('start() completed');
            }
        );
    }

    resume() {
        this.clearMessages();
        console.log('resume() called');
        let loader = this.presentLoader();
        this.dataService.resume().subscribe(
            data => {
                var resp = data.json();
                if (resp.Success) {
                    //this.infoMessage = `Geresumed`;
                    //this.setInfo(0, 'Geresumed');
                    this.presentToast(`Erfassung fortgesetzt`, 3000);
                    this.state = "started";
                    this.ellapsed = resp.TimeIntervall;
                }
                else {
                    this.errorMessage = 'resume Fehler: ' + resp.Error;
                    alert('Response not ok: ' + resp.Error);
                }
            },
            err => {
                this.errorMessage = "Fehler beim Fortsetzen";
                console.error(err);
            },
            () => {
                loader.dismiss();
                console.log('start() completed');
            }
        );
    }
}
