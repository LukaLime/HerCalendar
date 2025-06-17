import React, { useEffect, useState } from "react";
import ReactDOM from "react-dom/client";

const App = () => {
    const [cycles, setCycles] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetch("/api/cycles")
            .then((response) => {
                if (!response.ok) throw new Error("Failed to fetch cycles");
                return response.json();
            })
            .then((data) => {
                setCycles(data);
                setLoading(false);
            })
            .catch((error) => {
                console.error("Error fetching cycles:", error);
                setLoading(false);
            });
    }, []);

    const formatDate = (dateString) => {
        return new Date(dateString).toLocaleDateString();
    };

    if (loading) return <p>Loading...</p>;

    return (
        <div>
            <h2>My Cycle Tracker</h2>

            {cycles.length === 0 ? (
                <p>
                    No cycle data available.{" "}
                    <a href="/MyCycles/Create">Add your cycle information</a> to get started!
                </p>
            ) : (
                cycles.map((cycle) => (
                    <div key={cycle.id} className="card mb-3 p-3 shadow-sm">
                        <h4>Cycle Starting: {formatDate(cycle.lastPeriodStartDate)}</h4>
                        <p>
                            <strong>Next Period:</strong> {formatDate(cycle.nextPeriodStartDate)}
                        </p>
                        <p>
                            <strong>Cycle Length:</strong> {cycle.length} days
                        </p>
                        <a
                            href={`/MyCycles/Edit/${cycle.id}`}
                            className="btn btn-sm btn-warning me-2"
                        >
                            Edit
                        </a>
                        <a
                            href={`/MyCycles/Delete/${cycle.id}`}
                            className="btn btn-sm btn-danger"
                        >
                            Delete
                        </a>
                    </div>
                ))
            )}

            <a href="/MyCycles/Create" className="btn btn-primary mt-3">
                Add New Cycle
            </a>
        </div>
    );
};

const root = ReactDOM.createRoot(document.getElementById("react-root"));
root.render(<App />);




