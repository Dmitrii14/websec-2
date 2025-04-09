import React, { useState, useEffect, useRef, useCallback } from "react";
import { HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import ErrorMessage from "../ErrorMessage/ErrorMessage";
import environment from "../../config/environment.js";
import ListTopPlayers from "../ListTopPlayers/ListTopPlayers.jsx";
import UsernameForm from "../UsernameForm/UsernameForm.jsx";
import PlayerInformation from "../PlayerInformation/PlayerInformation.jsx";
import styles from "./GameCanvas.module.css";

const GameCanvas = () => {
  const [connection, setConnection] = useState(null);
  const [players, setPlayers] = useState({});
  const [topPlayers, setTopPlayers] = useState([]);
  const [star, setStar] = useState(null);
  const canvasRef = useRef(null);
  const pressedKeys = useRef(new Set());
  const [isJoined, setIsJoined] = useState(false);
  const [playerName, setPlayerName] = useState("");
  const [rating, setRating] = useState(0);
  const [shipImages, setShipImages] = useState({});
  const [starImage, setStarImage] = useState(null);
  const [isError, setIsError] = useState(false);

  const handleKeyDown = useCallback((event, conn) => {
    const directions = {
      ArrowUp: "up",
      ArrowDown: "down",
      ArrowLeft: "left",
      ArrowRight: "right",
      w: "up",
      s: "down",
      a: "left",
      d: "right",
    };

    if (directions[event.key]) {
      pressedKeys.current.add(directions[event.key]);
      sendMovement(conn);
    }
  }, []);

  const handleKeyUp = (event) => {
    const directions = {
      ArrowUp: "up",
      ArrowDown: "down",
      ArrowLeft: "left",
      ArrowRight: "right",
      w: "up",
      s: "down",
      a: "left",
      d: "right",
    };

    if (directions[event.key]) {
      pressedKeys.current.delete(directions[event.key]);
    }
  };

  const sendMovement = (conn) => {
    if (conn.state === HubConnectionState.Connected) {
      const movementArray = Array.from(pressedKeys.current);
      if (movementArray.length > 0) {
        conn.invoke("Move", movementArray);
      }
    }
  };

  const checkAndConnect = useCallback(async () => {
    if (connection) return;

    const newConnection = new HubConnectionBuilder()
      .withUrl(environment.backendUrl)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Error)
      .build();

    try {
      await newConnection.start();
      await newConnection.invoke("CheckGame");
      setConnection(newConnection);
      window.addEventListener("keydown", (event) => handleKeyDown(event, newConnection));
      window.addEventListener("keyup", handleKeyUp);
    } catch (error) {
      console.error("Connection failed: ", error);
    }
  }, [connection, handleKeyDown]);

  useEffect(() => {
    const star = new Image();
    star.src = "/star.png";
    setStarImage(star);
    const images = {};
    for (let i = 1; i <= 6; i++) {
      const img = new Image();
      img.src = `/${i}.png`;
      images[i] = img;
    }
    setShipImages(images);
  }, []);

  useEffect(() => {
    checkAndConnect();
  }, [checkAndConnect]);

  const handleJoin = async (name) => {
    if (connection) {
      try {
        await connection.invoke("RegisterPlayer", name);
        setPlayerName(name);
        setIsJoined(true);
      } catch (error) {
        console.error("Registration failed: ", error);
      }
    }
  };

  const handleLeave = useCallback(async () => {
    if (connection) {
      try {
        await connection.invoke("LeaveGame");
        setIsJoined(false);
        setPlayerName("");
        setRating(0);
      } catch (error) {
        console.error("Error exit: ", error);
      }
    }
  }, [connection]);


  useEffect(() => {
    if (!connection) return;

    connection.on("TopPlayers", (topPlayersData) => {
      setTopPlayers(topPlayersData);
    });

    connection.on("StarCollected", (starData) => {
      setStar(starData);
    });

    connection.on("ReceiveStars", (score) => {
      setRating(score);
    });

    connection.on("GameState", (playersData) => {
      setPlayers(playersData);
    });

    connection.on("Info", (status) => {
      if (status === "full") {
        setIsError(true);
        handleLeave();
      } else if (status === "empty") {
        setIsError(false);
      }
    });

    return () => connection.stop();
  }, [connection, handleLeave]);

  useEffect(() => {
    if (!canvasRef.current || Object.keys(shipImages).length === 0) return;
    const canvas = canvasRef.current;
    const ctx = canvas.getContext("2d");

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (star && starImage) {
      ctx.drawImage(starImage, star.x, star.y, 32, 32);
    }

    Object.values(players).forEach((player) => {
      const car = player.car;
      const carImage = shipImages[car.color];
      if (!carImage) return;

      const centerX = car.x + 20;
      const centerY = car.y + 20;

      ctx.save();
      ctx.translate(centerX, centerY);
      ctx.rotate(car.angle + Math.PI / 2);
      ctx.drawImage(carImage, -20, -20, 40, 40);
      ctx.restore();

      ctx.fillStyle = "#363947";
      ctx.font = "16px 'Press Start 2P', cursive";
      ctx.fillText(player.username, car.x, car.y - 5);
    });
  }, [players, star, shipImages, starImage]);

  return (
    <div className={styles.mainContainer}>
      <div className={styles.container}>
        <div>
          <canvas className={styles.field} ref={canvasRef} width={840} height={540} />
          <div className={`${styles.field} ${styles.inputContainer}`}>
            {!isJoined ? (
              <UsernameForm onSubmit={handleJoin} />
            ) : (
              <PlayerInformation playerName={playerName} starCount={rating} onLeave={handleLeave} />
            )}
            {isError && <ErrorMessage />}
          </div>
        </div>
        <div className={styles.listTopPlayersContainer}>
          <ListTopPlayers players={topPlayers} />
        </div>
      </div>
    </div>

  );
};

export default GameCanvas;