import React from 'react';
import InputField from '../../common/InputField'; 

/**
 * `AddPostInputField` renders input fields for adding or editing a post with details like title, description, address, area, job type, and dates.
 * 
 * Props:
 * - `title` (string): Title of the post.
 * - `description` (string): Description of the post.
 * - `address` (string): Address of the post location.
 * - `volunteerArea` (string): Area where volunteers are needed.
 * - `jobType` (string): Type of job available.
 * - `initialDate` (string): Starting date for the post.
 * - `lastDate` (string): Ending date for the post.
 * - `maxVolunteers` (string): Max number of volunteers for the post.
 * - `onChange` (function): Callback to handle changes in input values.
 * 
 * Usage:
 * ```jsx
 * <AddPostInputField
 *    title="Beach Cleanup"
 *    description="Help us clean the local beach."
 *    address="123 Ocean Drive"
 *    volunteerArea="South"
 *    jobType="Environment"
 *    initialDate="2024-01-01"
 *    lastDate="2024-01-02"
 *    onChange={handleInputChange}
 * />
 * ```
 */

const AddPostInputField = ({ title, description, address, volunteerArea, jobType, initialDate, lastDate, maxVolunteers, onChange }) => {
  return (
    <div className="add-post-input-fields">
      <InputField label="Title:" type="text" name="title" value={title} onChange={onChange} required />
      <InputField label="Description:" multiline name="description" value={description} onChange={onChange} required rows="10" />
      <InputField label="Address:" type="text" name="address" value={address} onChange={onChange} required />
      <InputField label="Area:" type="select" name="volunteerArea" value={volunteerArea} onChange={onChange} options={[
        { label: "Select Area", value: "" },
        { label: "North", value: "North" },
        { label: "South", value: "South" },
        { label: "Central", value: "Central" }
      ]} />
      <InputField label="Job Type:" type="select" name="jobType" value={jobType} onChange={onChange} options={[
        { label: "Select Job Type", value: "" },
        { label: "Agriculture", value: "Agriculture" },
        { label: "Cooking", value: "Cooking" },
        { label: "Transportation", value: "Transportation" },
        { label: "Animal Care", value: "AnimalCare" }
      ]} />
      <InputField label="Initial Date:" type="date" name="initialDate" value={initialDate} onChange={onChange} />
      <InputField label="End Date:" type="date" name="lastDate" value={lastDate} onChange={onChange} />
      <InputField label="Max Volunteers:" type="text" name="maxVolunteers" value={maxVolunteers} onChange={onChange} 
        placeholder="Enter max volunteers or leave empty for no limit" />
    </div>
  );
};

export default AddPostInputField;
