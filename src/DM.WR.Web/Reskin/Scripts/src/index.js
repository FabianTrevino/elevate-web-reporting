import React from "react";
import ReactDOM from "react-dom";
import ElevateHeader from "./components/ElevateHeader";
import "./assets/css/elevate.scss";
//import "elevate-react-lib/dist/assets/css/index.scss";

if (process.env.NODE_ENV === "production") {
  console.log("Production mode!");
}
if (process.env.NODE_ENV !== "production") {
    console.log("Development mode!");
    console.log("AAA");
}

const App = () => <ElevateHeader/>;

ReactDOM.render(<App />, document.getElementById("root"));
