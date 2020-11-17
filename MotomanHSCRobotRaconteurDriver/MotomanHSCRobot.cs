using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotRaconteur;
using RobotRaconteur.Companion.Robot;
using com.robotraconteur.robotics.robot;
using com.robotraconteur.robotics.joints;
using com.robotraconteur.geometry;
using com.robotraconteur.action;
using com.robotraconteur.robotics.trajectory;
using MotomanCATSInterfaceLibrary;
using System.Threading.Tasks;

namespace MotomanHSCRobotRaconteurDriver
{
    class MotomanHSCRobot : AbstractRobot
    {
        MotomanCATSInterface _motoman_hsc;
        public MotomanHSCRobot(com.robotraconteur.robotics.robot.RobotInfo robot_info) : base(robot_info, -1)
        {
            _uses_homing = false;
            _has_position_command = true;
            _has_velocity_command = false;
            _update_period = 2;
            robot_info.robot_capabilities &= (uint)(RobotCapabilities.jog_command & RobotCapabilities.position_command & RobotCapabilities.trajectory_command);

        }

        public override void _start_robot()
        {
            _motoman_hsc = new MotomanCATSInterface();
            _motoman_hsc.Start();
            base._start_robot();
        }

        protected override Task _send_disable()
        {
            throw new NotImplementedException();
        }

        protected override Task _send_enable()
        {
            throw new NotImplementedException();
        }

        protected override Task _send_reset_errors()
        {
            throw new NotImplementedException();
        }

        protected override void _send_robot_command(long now, double[] joint_pos_cmd, double[] joint_vel_cmd)
        {
            if (joint_pos_cmd != null)
            {
                //egm_client.SetCommandPosition(joint_pos_cmd);
            }
        }

        protected override void _run_timestep(long now)
        {
            _motoman_hsc.Update();
            // TODO: Need to get operational mode!
            _operational_mode = RobotOperationalMode.auto;
            _last_joint_state = now;
            _last_endpoint_state = now;
            _last_robot_state = now;
            uint system_status = _motoman_hsc.SystemStatus;

            _enabled = true;
            _ready = true;
            double[] act_joint_angles = _motoman_hsc.ActualJointAngles;
            act_joint_angles[8] -= Math.PI / 180.0;
            act_joint_angles[11] -= Math.PI / 180.0;
            act_joint_angles[1] -= Math.PI / 180.0;
            act_joint_angles[4] += Math.PI / 180.0;
            _joint_position = act_joint_angles;
            
                        
            _endpoint_pose = new Pose[] { };

            base._run_timestep(now);

            if (_command_mode == RobotCommandMode.halt || _command_mode == RobotCommandMode.invalid_state)
            {
                // do nothing. Can we halt the motoman?
            }
        }

        public override void Dispose()
        {
            _motoman_hsc?.Shutdown();
            base.Dispose();
        }
    }
}
