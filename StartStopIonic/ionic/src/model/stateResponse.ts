/* * * ./app/model/stateResponse.ts * * */
export class StateResponse {
    constructor(
        public Success: boolean, 
        public Error: string,
        public TimeIntervall: string
    ){}
}